﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Caboodle;
using RuntimeDeviceType = Microsoft.Caboodle.DeviceType;
using XUnitFilter = UnitTests.HeadlessRunner.Xunit.XUnitFilter;

namespace Caboodle.DeviceTests
{
    internal static class Traits
    {
        public const string DeviceType = "DeviceType";

        internal static class DeviceTypes
        {
            public const string Physical = "Physical";
            public const string Virtual = "Virtual";

            internal static string ToExclude =>
                DeviceInfo.DeviceType == RuntimeDeviceType.Physical ? Virtual : Physical;
        }

        internal static List<XUnitFilter> GetCommonTraits(params XUnitFilter[] additionalFilters)
        {
            var filters = new List<XUnitFilter>
            {
                new XUnitFilter(DeviceType, DeviceTypes.ToExclude, true)
            };

            if (additionalFilters != null && additionalFilters.Any())
                filters.AddRange(additionalFilters);

            return filters;
        }
    }
}
