using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SharedSession
{
    [ComVisible(true), GuidAttribute("B68B49DA-108C-4EDC-893B-0DAAC7F0514F")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISession
    {
        void Open(dynamic request, dynamic session);
        void Save(dynamic request, dynamic response, dynamic session);
    }
}
