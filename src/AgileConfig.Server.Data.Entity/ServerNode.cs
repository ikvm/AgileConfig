﻿using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using AgileConfig.Server.Common;

namespace AgileConfig.Server.Data.Entity
{
    /// <summary>
     ///   Online = 1,
     ///   Offline = 0,
    /// </summary>
    public enum NodeStatus
    {
        Online = 1,
        Offline = 0,
    }

    [Table(Name = "agc_server_node")]
    [OraclePrimaryKeyName("agc_server_node_pk")]
    public class ServerNode: IEntity<string>
    {
        [BsonId]
        [Column(Name = "address", StringLength = 100, IsPrimary = true)]
        public string Id { get; set; }

        [Column(Name = "remark", StringLength = 50)]
        public string Remark { get; set; }

        [Column(Name = "status")]
        public NodeStatus Status { get; set; }

        [Column(Name = "last_echo_time")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastEchoTime { get; set; }

        [Column(Name = "create_time")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
    }
}
