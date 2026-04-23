using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Entity
{
    public interface IEntity
    {
        long Id { get; }
        string Codigo { get; set; }
    }
}
