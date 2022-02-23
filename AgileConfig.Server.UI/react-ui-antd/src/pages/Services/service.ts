import request from '@/utils/request';

export async function queryService(params: any) {
  return request('/service/search', {
    params
  });
}
 
export async function reloadClientConfigs(address: string, clientId: string) {
  return request('/RemoteServerProxy/Client_Reload', {
    method: 'POST',
    params:{
      address,
      clientId
    }
  });
}
export async function clientOffline(address: string, clientId: string) {
  return request('/RemoteServerProxy/Client_Offline', {
    method: 'POST',
    params:{
      address,
      clientId
    }
  });
}