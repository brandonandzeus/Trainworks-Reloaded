using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TrainworksReloaded.Base.Extensions
{
    public static class ParseReferenceExtensions
    {
        public class ReferencedObject
        {
            public string id;
            public string? mod_reference;

            public ReferencedObject(string id, string? mod_reference)
            {
                this.id = id;
                this.mod_reference = mod_reference;
            }

            public string ToId(string defaultKey, string template)
            {
                var key = mod_reference ?? defaultKey;
                return id.ToId(key, template);
            }
        }

        public static ReferencedObject? ParseReference(this IConfigurationSection section)
        {
            string? id = section.Value ?? section.GetSection("id").Value;
            string? mod_reference = section.GetSection("mod_reference").Value;
            if (id == null)
                return null;
            return new ReferencedObject(id, mod_reference);
        }
    }
}
