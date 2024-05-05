﻿using Agile.Config.Protocol;
using AgileConfig.Server.Common;
using AgileConfig.Server.Common.EventBus;
using AgileConfig.Server.Common.RestClient;
using AgileConfig.Server.Data.Entity;
using AgileConfig.Server.Event;
using AgileConfig.Server.IService;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AgileConfig.Server.EventHandler
{
    public class ServiceRegisterHandler : IEventHandler<ServiceRegisteredEvent>
    {
        private readonly IRemoteServerNodeProxy _remoteServerNodeProxy;
        private readonly IServerNodeService _serverNodeService;

        public ServiceRegisterHandler(
            IRemoteServerNodeProxy remoteServerNodeProxy,
            IServerNodeService serverNodeService
            )
        {
            _remoteServerNodeProxy = remoteServerNodeProxy;
            _serverNodeService = serverNodeService;
        }

        public async Task Handle(IEvent evt)
        {
            var serverNodes = await _serverNodeService.GetAllNodesAsync();
            foreach (var serverNode in serverNodes.Where(x => x.Status == NodeStatus.Online))
            {
                //clear cache
                _ = _remoteServerNodeProxy.ClearServiceInfoCache(serverNode.Id);
                //send ws action
                var act = new WebsocketAction()
                {
                    Module = ActionModule.RegisterCenter,
                    Action = ActionConst.Reload
                };
                _ = _remoteServerNodeProxy.AllClientsDoActionAsync(serverNode.Id, act);
            }
        }
    }

    public class ServiceUnRegisterHandler : IEventHandler<ServiceUnRegisterEvent>
    {
        private readonly IRemoteServerNodeProxy _remoteServerNodeProxy;
        private readonly IServerNodeService _serverNodeService;

        public ServiceUnRegisterHandler(
            IRemoteServerNodeProxy remoteServerNodeProxy,
            IServerNodeService serverNodeService
            )
        {
            _remoteServerNodeProxy = remoteServerNodeProxy;
            _serverNodeService = serverNodeService;
        }

        public async Task Handle(IEvent evt)
        {
            var serverNodes = await _serverNodeService.GetAllNodesAsync();
            foreach (var serverNode in serverNodes.Where(x => x.Status == NodeStatus.Online))
            {
                //clear cache
                _ = _remoteServerNodeProxy.ClearServiceInfoCache(serverNode.Id);
                //send ws action
                var act = new WebsocketAction()
                {
                    Module = ActionModule.RegisterCenter,
                    Action = ActionConst.Reload
                };
                _ = _remoteServerNodeProxy.AllClientsDoActionAsync(serverNode.Id, act);
            }
        }
    }

    public class ServiceStatusUpdateHandler : IEventHandler<ServiceStatusUpdateEvent>
    {
        private readonly IRemoteServerNodeProxy _remoteServerNodeProxy;
        private readonly IRestClient _restClient;
        private ILogger _logger;
        private readonly IServerNodeService _serverNodeService;
        private readonly IServiceInfoService _serviceInfoService;

        public ServiceStatusUpdateHandler(
            IRemoteServerNodeProxy remoteServerNodeProxy,
            ILoggerFactory loggerFactory,
            IRestClient restClient,
            IServerNodeService serverNodeService,
            IServiceInfoService serviceInfoService
            )
        {
            _remoteServerNodeProxy = remoteServerNodeProxy;
            _restClient = restClient;
            _serverNodeService = serverNodeService;
            _serviceInfoService = serviceInfoService;
            _logger = loggerFactory.CreateLogger<ServiceStatusUpdateHandler>();
        }

        public async Task Handle(IEvent evt)
        {
            var serverNodes = await _serverNodeService.GetAllNodesAsync();
            foreach (var serverNode in serverNodes.Where(x => x.Status == NodeStatus.Online))
            {
                //clear cache
                _ = _remoteServerNodeProxy.ClearServiceInfoCache(serverNode.Id);
                //send ws action
                var act = new WebsocketAction()
                {
                    Module = ActionModule.RegisterCenter,
                    Action = ActionConst.Reload
                };
                _ = _remoteServerNodeProxy.AllClientsDoActionAsync(serverNode.Id, act);
            }

            var evtInstance = evt as ServiceStatusUpdateEvent;
            string id = evtInstance?.UniqueId;
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            var service = await _serviceInfoService.GetByUniqueIdAsync(id);
            if (service != null && !string.IsNullOrWhiteSpace(service.AlarmUrl) &&
                service.Status == ServiceStatus.Unhealthy)
            {
                //如果是下线发送通知
                _ = SendServiceOfflineMessageAsync(service);
            }
        }

        private async Task SendServiceOfflineMessageAsync(ServiceInfo service)
        {
            var msg = new
            {
                UniqueId = service.Id,
                service.ServiceId,
                service.ServiceName,
                Time = DateTime.Now,
                Status = ServiceStatus.Unhealthy.ToString(),
                Message = "服务不健康"
            };

            try
            {
                await FunctionUtil.TRYAsync(async () =>
                {
                    var content = new StringContent("");
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    using var resp = await _restClient.PostAsync(service.AlarmUrl, null);

                    resp.EnsureSuccessStatusCode();

                    return resp.StatusCode == HttpStatusCode.OK;
                }, 5);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"try to send message to alarm url {service.AlarmUrl} but failed");
            }
        }
    }
}
