using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;

namespace Capstone_SWP490.Helper
{
    public class ResourceManagerHelper
    {
        private ResourceManager rm;
        public ResourceManagerHelper(string resourceName)
        {
            var baseName =  resourceName;
            rm = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
        }
        public string getString(string key)
        {
            try
            {
                return rm.GetString(key);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}