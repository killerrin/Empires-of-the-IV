using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Interfaces
{
    public interface ISelectableEntity
    {
        bool Selected { get; set; }
        bool Selectable { get; set; }
    }
}
