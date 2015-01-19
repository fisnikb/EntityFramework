// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Metadata.ModelConventions
{
    public class ConventionsDispatcher
    {
        public ConventionsDispatcher()
        {
            OnEntityTypeAddedConventions = new List<IEntityTypeConvention>();
            OnForeignKeyAddedConventions = new List<IRelationshipConvention>();
        }

        public virtual IList<IEntityTypeConvention> OnEntityTypeAddedConventions { get; }

        public virtual IList<IRelationshipConvention> OnForeignKeyAddedConventions { get; }

        public virtual InternalEntityBuilder OnEntityTypeAdded([NotNull] InternalEntityBuilder entityBuilder)
        {
            Check.NotNull(entityBuilder, "entityBuilder");

            foreach (var entityTypeConvention in OnEntityTypeAddedConventions)
            {
                entityBuilder = entityTypeConvention.Apply(entityBuilder);
                if (entityBuilder == null)
                {
                    break;
                }
            }

            return entityBuilder;
        }

        public virtual InternalRelationshipBuilder OnRelationshipAdded([NotNull] InternalRelationshipBuilder relationshipBuilder)
        {
            Check.NotNull(relationshipBuilder, "relationshipBuilder");

            foreach (var relationshipConvention in OnForeignKeyAddedConventions)
            {
                relationshipBuilder = relationshipConvention.Apply(relationshipBuilder);
                if (relationshipBuilder == null)
                {
                    break;
                }
            }

            return relationshipBuilder;
        }
    }
}
