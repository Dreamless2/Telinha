using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Entity
{
    public interface IEntity
    {
        public long Id { get; }
        public string Codigo { get; set; }
    }
}
