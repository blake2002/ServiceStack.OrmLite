﻿using NUnit.Framework;
using ServiceStack.DataAnnotations;
using ServiceStack.Text;

namespace ServiceStack.OrmLite.Tests.Issues
{
    [Schema("schema1")]
    public class Entity1
    {
        public int Id { get; set; }

        public int Entity2Fk { get; set; }
    }

    [Schema("schema1")]
    public class Entity2
    {
        public int Id { get; set; }
    }

    public class PlainModel
    {
        public int Entity1Id { get; set; }

        public int Entity2Id { get; set; }
    }

    public class JoinsWithSchemas : OrmLiteTestBase
    {
        [Test]
        public void Can_join_entities_with_Schema()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<Entity1>();
                db.DropAndCreateTable<Entity2>();

                db.Insert(new Entity2 { Id = 1 });
                db.Insert(new Entity1 { Id = 2, Entity2Fk = 1 });

                var results = db.Select<PlainModel>(
                    db.From<Entity1>()
                      .Join<Entity2>((e1, e2) => e1.Entity2Fk == e2.Id));

                Assert.That(results.Count, Is.EqualTo(1));
                Assert.That(results[0].Entity1Id, Is.EqualTo(2));
                Assert.That(results[0].Entity2Id, Is.EqualTo(1));
            }
        }
    }
}