// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Metadata
{
    public static class EntityTypeExtensions
    {
        public static IEnumerable<IPropertyBase> GetPropertiesAndNavigations([NotNull] this IEntityType entityType)
        {
            Check.NotNull(entityType, "entityType");

            return entityType.Properties.Concat<IPropertyBase>(entityType.Navigations);
        }

        [NotNull]
        public static IEnumerable<IForeignKey> GetReferencingForeignKeys([NotNull] this IEntityType entityType)
        {
            return entityType.Model.GetReferencingForeignKeys(entityType);
        }

        public static bool HasPropertyChangingNotifications([NotNull] this IEntityType entityType)
        {
            return entityType.Type == null
                   || typeof(INotifyPropertyChanging).GetTypeInfo().IsAssignableFrom(entityType.Type.GetTypeInfo());
        }

        public static bool HasPropertyChangedNotifications([NotNull] this IEntityType entityType)
        {
            return entityType.Type == null
                   || typeof(INotifyPropertyChanged).GetTypeInfo().IsAssignableFrom(entityType.Type.GetTypeInfo());
        }

        public static ForeignKey TryGetForeignKey(
            [NotNull] this EntityType dependentType,
            [NotNull] EntityType principalType,
            [CanBeNull] string navigationToPrincipal,
            [CanBeNull] string navigationToDependent,
            [CanBeNull] IReadOnlyList<Property> foreignKeyProperties,
            [CanBeNull] IReadOnlyList<Property> referencedProperties,
            bool? isUnique)
        {
            Check.NotNull(dependentType, "dependentType");
            Check.NotNull(principalType, "principalType");

            return dependentType.ForeignKeys.FirstOrDefault(fk =>
                fk.IsCompatible(
                    principalType,
                    dependentType,
                    navigationToPrincipal,
                    navigationToDependent,
                    foreignKeyProperties,
                    referencedProperties,
                    isUnique));
        }
    }
}
