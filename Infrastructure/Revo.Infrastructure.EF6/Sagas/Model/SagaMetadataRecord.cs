﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revo.DataAccess.Entities;

namespace Revo.Infrastructure.EF6.Sagas.Model
{
    [TablePrefix(NamespacePrefix = "REV", ColumnPrefix = "SMR")]
    [DatabaseEntity]
    public class SagaMetadataRecord
    {
        public SagaMetadataRecord(Guid id, Guid classId)
        {
            Id = id;
            ClassId = classId;
        }

        protected SagaMetadataRecord()
        {
        }

        public Guid Id { get; private set; }
        public Guid ClassId { get; private set; }
        public List<SagaMetadataKey> Keys { get; private set; } = new List<SagaMetadataKey>();
    }
}
