﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.WebJob.Models
{
    public class DeploymentOutPut
    {
       Parameter parameter { get; set; }
    }

    public class Parameter
    {
        public string type { get; set; }
        public string value { get; set; }

    }
}