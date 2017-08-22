using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Validation;
using BriefFiniteElementNet;

namespace BriefFiniteElementNet.TestConsole
{
    class Class2
    {
        public void Check()
        {
            var model=new Model();

            var wnd = WpfTraceListener.CreateModelTrace(model);
            PosdefChecker.CheckModel(model, LoadCase.DefaultLoadCase);
            wnd.ShowDialog();
        }
    }
}
