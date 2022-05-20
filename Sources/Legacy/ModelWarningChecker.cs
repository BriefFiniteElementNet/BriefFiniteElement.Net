using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Legacy
{
    public class ModelWarningChecker
    {
        /// <summary>
        /// Checks the model for warnings.
        /// </summary>
        /// <param name="model">The model.</param>
        public void CheckModel(Model model)
        {
            for (var i = 0; i < model.Elements.Count; i++)
            {
                var elementIdentifier = string.Format("{0}'th element in Model.Elements", i);

                if (model.Elements[i] is FrameElement2Node)
                {
                    var elm = model.Elements[i] as FrameElement2Node;
                    if (elm.Label != null)
                        elementIdentifier = elm.Label;

                    //10000
                    if (elm.Geometry != null && elm.UseOverridedProperties)
                    {
                        var rec = TraceRecords.GetRecord(10000, elementIdentifier);
                        model.Trace.Write(rec);
                    }


                    //10010
                    if (elm.Geometry == null && (elm.A == 0 || elm.Iy == 0 || elm.Iz == 0))
                    {
                        var rec = TraceRecords.GetRecord(10010, elementIdentifier);
                        model.Trace.Write(rec);
                    }


                    //10020
                    if (elm.ConsiderShearDeformation && elm.Geometry == null && (elm.Ay == 0 || elm.Az == 0))
                    {
                        var rec = TraceRecords.GetRecord(10020, elementIdentifier);
                        model.Trace.Write(rec);
                    }

                    //10100
                    if (elm.G == 0 || elm.E == 0)
                    {
                        var rec = TraceRecords.GetRecord(10100, elementIdentifier);
                        model.Trace.Write(rec);
                    }
                }


                if (model.Elements[i] is TrussElement2Node)
                {
                    var elm = model.Elements[i] as TrussElement2Node;
                    if (elm.Label != null)
                        elementIdentifier = elm.Label;

                    //10000
                    if (elm.Geometry != null && elm.UseOverridedProperties)
                    {
                        var rec = TraceRecords.GetRecord(10000, elementIdentifier);
                        model.Trace.Write(rec);
                    }


                    //10010
                    if (elm.Geometry == null && (elm.A == 0))
                    {
                        var rec = TraceRecords.GetRecord(10010, elementIdentifier);
                        model.Trace.Write(rec);
                    }


                    //10100
                    if (elm.E == 0)
                    {
                        var rec = TraceRecords.GetRecord(10100, elementIdentifier);
                        model.Trace.Write(rec);
                    }
                }
            }
        }



    }
}