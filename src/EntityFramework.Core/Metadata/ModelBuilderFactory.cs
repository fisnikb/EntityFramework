// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Metadata.ModelConventions;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Metadata
{
    public class ModelBuilderFactory : IModelBuilderFactory
    {
        public virtual ModelBuilder CreateConventionBuilder(Model model)
        {
            Check.NotNull(model, "model");

            return new ModelBuilder(model, CreateConventionsDispatcher());
        }

        protected virtual ConventionsDispatcher CreateConventionsDispatcher()
        {
            var conventions = new ConventionsDispatcher();

            conventions.OnEntityTypeAddedConventions.Add(new PropertiesConvention());
            conventions.OnEntityTypeAddedConventions.Add(new KeyConvention());
            conventions.OnEntityTypeAddedConventions.Add(new RelationshipDiscoveryConvention());

            conventions.OnForeignKeyAddedConventions.Add(new ForeignKeyPropertyDiscoveryConvention());

            return conventions;
        }
    }
}
