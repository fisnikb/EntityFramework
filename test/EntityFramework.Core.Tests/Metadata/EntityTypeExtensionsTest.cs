// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity.Metadata;
using Moq;
using Xunit;

namespace Microsoft.Data.Entity.Tests.Metadata
{
    public class EntityTypeExtensionsTest
    {
        private readonly Model _model = BuildModel();

        [Fact]
        public void Can_get_all_properties_and_navigations()
        {
            var typeMock = new Mock<IEntityType>();

            var property1 = Mock.Of<IProperty>();
            var property2 = Mock.Of<IProperty>();
            var navigation1 = Mock.Of<INavigation>();
            var navigation2 = Mock.Of<INavigation>();

            typeMock.Setup(m => m.Properties).Returns(new List<IProperty> { property1, property2 });
            typeMock.Setup(m => m.Navigations).Returns(new List<INavigation> { navigation1, navigation2 });

            Assert.Equal(
                new IPropertyBase[] { property1, property2, navigation1, navigation2 },
                typeMock.Object.GetPropertiesAndNavigations().ToArray());
        }

        [Fact]
        public void Can_get_referencing_foreign_keys()
        {
            var modelMock = new Mock<Model>();
            var entityType = new EntityType("Customer", modelMock.Object);

            entityType.GetReferencingForeignKeys();

            modelMock.Verify(m => m.GetReferencingForeignKeys(entityType), Times.Once());
        }

        [Fact]
        public void Foreign_key_matching_principal_type_name_plus_PK_name_is_found()
        {
            var fkProperty = DependentType.GetOrAddProperty("PrincipalEntityPeEKaY", typeof(int), shadowProperty: true);

            var fk = DependentType.GetOrAddForeignKey(fkProperty, PrincipalType.GetPrimaryKey());

            Assert.Same(fk, DependentType.TryGetForeignKey(
                PrincipalType,
                "SomeNav",
                "SomeInverse",
                null,
                null,
                isUnique: false));
        }

        [Fact]
        public void Foreign_key_matching_given_properties_is_found()
        {
            DependentType.GetOrAddProperty("SomeNavID", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("SomeNavPeEKaY", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityID", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityPeEKaY", typeof(int), shadowProperty: true);
            var fkProperty = DependentType.GetOrAddProperty("HeToldMeYouKilledMyFk", typeof(int), shadowProperty: true);

            var fk = DependentType.GetOrAddForeignKey(fkProperty, PrincipalType.GetPrimaryKey());

            Assert.Same(
                fk,
                DependentType.TryGetForeignKey(
                    PrincipalType,
                    "SomeNav",
                    "SomeInverse",
                    new[] { fkProperty },
                    new Property[0],
                    isUnique: false));
        }

        [Fact]
        public void Foreign_key_matching_given_property_is_found()
        {
            DependentType.GetOrAddProperty("SomeNavID", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("SomeNavPeEKaY", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityID", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityPeEKaY", typeof(int), shadowProperty: true);
            var fkProperty1 = DependentType.GetOrAddProperty("No", typeof(int), shadowProperty: true);
            var fkProperty2 = DependentType.GetOrAddProperty("IAmYourFk", typeof(int), shadowProperty: true);

            var fk = DependentType.GetOrAddForeignKey(new[] { fkProperty1, fkProperty2 }, PrincipalType.GetOrAddKey(
                new[]
                    {
                        PrincipalType.GetOrAddProperty("Id1", typeof(int), shadowProperty: true),
                        PrincipalType.GetOrAddProperty("Id2", typeof(int), shadowProperty: true)
                    }));

            Assert.Same(
                fk,
                DependentType.TryGetForeignKey(
                    PrincipalType,
                    "SomeNav",
                    "SomeInverse",
                    new[] { fkProperty1, fkProperty2 },
                    new Property[0],
                    isUnique: false));
        }

        [Fact]
        public void Foreign_key_matching_navigation_plus_Id_is_found()
        {
            var fkProperty = DependentType.GetOrAddProperty("SomeNavID", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("SomeNavPeEKaY", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityID", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityPeEKaY", typeof(int), shadowProperty: true);

            var fk = DependentType.GetOrAddForeignKey(fkProperty, PrincipalType.GetPrimaryKey());

            Assert.Same(
                fk,
                DependentType.TryGetForeignKey(
                    PrincipalType,
                    "SomeNav",
                    "SomeInverse",
                    null,
                    null,
                    isUnique: false));
        }

        [Fact]
        public void Foreign_key_matching_navigation_plus_PK_name_is_found()
        {
            var fkProperty = DependentType.GetOrAddProperty("SomeNavPeEKaY", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityID", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityPeEKaY", typeof(int), shadowProperty: true);

            var fk = DependentType.GetOrAddForeignKey(fkProperty, PrincipalType.GetPrimaryKey());

            Assert.Same(
                fk,
                DependentType.TryGetForeignKey(
                    PrincipalType,
                    "SomeNav",
                    "SomeInverse",
                    null,
                    null,
                    isUnique: false));
        }

        [Fact]
        public void Foreign_key_matching_principal_type_name_plus_Id_is_found()
        {
            var fkProperty = DependentType.GetOrAddProperty("PrincipalEntityID", typeof(int), shadowProperty: true);
            DependentType.GetOrAddProperty("PrincipalEntityPeEKaY", typeof(int), shadowProperty: true);

            var fk = DependentType.GetOrAddForeignKey(fkProperty, PrincipalType.GetPrimaryKey());

            Assert.Same(
                fk,
                DependentType.TryGetForeignKey(
                    PrincipalType,
                    "SomeNav",
                    "SomeInverse",
                    null,
                    null,
                    isUnique: false));
        }

        [Fact]
        public void Does_not_match_existing_FK_if_FK_has_different_navigation_to_principal()
        {
            var fkProperty = DependentType.GetOrAddProperty("SharedFk", typeof(int), shadowProperty: true);
            var fk = DependentType.GetOrAddForeignKey(fkProperty, PrincipalType.GetPrimaryKey());
            DependentType.AddNavigation("AnotherNav", fk, pointsToPrincipal: true);

            var newFk = DependentType.TryGetForeignKey(
                PrincipalType,
                "SomeNav",
                "SomeInverse",
                new[] { fkProperty },
                new Property[0],
                isUnique: false);

            Assert.Null(newFk);
        }

        [Fact]
        public void Does_not_match_existing_FK_if_FK_has_different_navigation_to_dependent()
        {
            var fkProperty = DependentType.GetOrAddProperty("SharedFk", typeof(int), shadowProperty: true);
            var fk = DependentType.GetOrAddForeignKey(fkProperty, PrincipalType.GetPrimaryKey());
            PrincipalType.AddNavigation("AnotherNav", fk, pointsToPrincipal: false);

            var newFk = DependentType.TryGetForeignKey(
                PrincipalType,
                "SomeNav",
                "SomeInverse",
                new[] { fkProperty },
                new Property[0],
                isUnique: false);

            Assert.Null(newFk);
        }

        [Fact]
        public void Does_not_match_existing_FK_if_FK_has_different_uniqueness()
        {
            var fkProperty = DependentType.GetOrAddProperty("SharedFk", typeof(int), shadowProperty: true);
            var fk = DependentType.GetOrAddForeignKey(fkProperty, PrincipalType.GetPrimaryKey());
            fk.IsUnique = true;

            var newFk = DependentType.TryGetForeignKey(
                PrincipalType,
                "SomeNav",
                "SomeInverse",
                new[] { fkProperty },
                new Property[0],
                isUnique: false);

            Assert.Null(newFk);
        }

        private static Model BuildModel()
        {
            var model = new Model();

            var principalType = model.AddEntityType(typeof(PrincipalEntity));
            principalType.GetOrSetPrimaryKey(principalType.GetOrAddProperty("PeeKay", typeof(int)));

            var dependentType = model.AddEntityType(typeof(DependentEntity));
            dependentType.GetOrSetPrimaryKey(dependentType.GetOrAddProperty("KayPee", typeof(int), shadowProperty: true));

            return model;
        }

        private EntityType DependentType
        {
            get { return _model.GetEntityType(typeof(DependentEntity)); }
        }

        private EntityType PrincipalType
        {
            get { return _model.GetEntityType(typeof(PrincipalEntity)); }
        }

        private class PrincipalEntity
        {
            public int PeeKay { get; set; }
            public IEnumerable<DependentEntity> AnotherNav { get; set; }
        }

        private class DependentEntity
        {
            public PrincipalEntity Navigator { get; set; }
            public PrincipalEntity AnotherNav { get; set; }
        }
    }
}
