using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using System;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represent a tool for checking the model warnings that probably leads to error in solve time
    /// </summary>
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
                var elementIdentifier = string.Format("{0}'th element in Model.Elements",i);

            }
        }


       
    }

    /// <summary>
    /// Represents ready records for well known situations
    /// </summary>
    public static class TraceRecords
    {
        /// <summary>
        /// Gets the ready filled record.
        /// </summary>
        /// <param name="errorNumber">The error Number.</param>
        /// <param name="identifier">The element identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static Common.TraceRecord GetRecord(int errorNumber, string identifier)
        {
            var rec = new TraceRecord();
            rec.TimeStamp = DateTime.Now;

            rec.HelpLink = @"https://brieffiniteelementnet.codeplex.com/wikipage?title=Error%20message%20list"
                           + "#MA" + errorNumber;

            rec.TargetIdentifier = identifier;

            switch (errorNumber)
            {
                case 10000:
                    rec.IssueId = "MA10000";
                    rec.Level = TraceLevel.Warning;
                    rec.Message = "FrameElement2Node.Geometry & FrameElement2Node.UseOverridedProperties values are inconsistent";
                    break;

                case 10010:
                    rec.IssueId = "MA10010";
                    rec.Level = TraceLevel.Warning;
                    rec.Message = "Neither FrameElement2Node.Geometry and one of {FrameElement2Node.A,FrameElement2Node.Iy,FrameElement2Node.Iz} values are set";
                    break;

                case 10020:
                    rec.IssueId = "MA10020";
                    rec.Level = TraceLevel.Warning;
                    rec.Message = "FrameElement2Node.ConsiderShearDeformation & FrameElement2Node.Ay and FrameElement2Node.Az values are inconsistent";
                    break;

                case 10100:
                    rec.IssueId = "MA10100";
                    rec.Level = TraceLevel.Warning;
                    rec.Message = "Either FrameElement2Node.E or FrameElement2Node.G is not set";
                    break;

                case 30000:
                    rec.IssueId = "MA30000";
                    rec.Level = TraceLevel.Error;
                    rec.Message = "DoF is not properly restrained";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("errorNumber");
            }

            return rec;
        }
    }
}
