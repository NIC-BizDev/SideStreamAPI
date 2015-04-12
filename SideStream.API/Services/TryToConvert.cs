using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.Services
{
    public static class TryToConvert
    {
        public static int? ToInt(object value)
        {
            int? retval = null;
            if (value != null)
            {
                var str = value.ToString();
                if (!String.IsNullOrWhiteSpace(str))
                {
                    try
                    {
                        retval = Convert.ToInt32(str);
                    }
                    catch
                    {
                        retval = null;
                    }
                }
            }
            return retval;
        }
    }
}