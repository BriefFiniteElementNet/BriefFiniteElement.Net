using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Markup;
using System.Xaml;
using System.IO;

namespace BriefFiniteElementNet.XamlSerialization
{
    public class XamlSerializer
    {
        public static void Serialize(Model model)
        {
            var memStr = new System.IO.MemoryStream();

            Register<Point, PtConverter>();
            Register<Displacement, DisplacementConverter>();
            Register<Constraint, DisplacementConverter>();

            var cnv = TypeDescriptor.GetConverter(typeof(Point));


            var nde = new Node();

            //System.Windows.Markup.XamlWriter.Save(nde, memStr);
            System.Windows.Markup.XamlWriter.Save(model, memStr);


            memStr.Position = 0;

            var rdr = new StreamReader(memStr);

            var data = rdr.ReadToEnd();

        }


        public static void Register<T, TC>() where TC : TypeConverter
        {
            Attribute[] attr = new Attribute[1];
            TypeConverterAttribute vConv = new TypeConverterAttribute(typeof(TC));
            attr[0] = vConv;
            TypeDescriptor.AddAttributes(typeof(T), attr);
        }

        public class PtConverter : TypeConverter
        {
            public override bool CanConvertFrom(
         System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }
            //should return true when destinationtype if GeopointItem
            public override bool CanConvertTo(
                 System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }



            public override object ConvertTo(
         System.ComponentModel.ITypeDescriptorContext context,
          System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == null)
                    throw new ArgumentNullException("destinationType");

                Point gpoint = (Point)value;

                if (gpoint != null)
                    if (this.CanConvertTo(context, destinationType))
                        return gpoint.ToString();

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }


        public class DisplacementConverter : TypeConverter
        {
            public override bool CanConvertFrom(
         System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }
            //should return true when destinationtype if GeopointItem
            public override bool CanConvertTo(
                 System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }



            public override object ConvertTo(
         System.ComponentModel.ITypeDescriptorContext context,
          System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == null)
                    throw new ArgumentNullException("destinationType");

                var gpoint = (object)value;

                if (gpoint != null)
                    if (this.CanConvertTo(context, destinationType))
                        return gpoint.ToString();

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
