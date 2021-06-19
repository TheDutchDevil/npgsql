using System;
using System.Data;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class NpgsqlBatchTests : MultiplexingTestBase
    {
        [Test]
        public async Task Named_parameters()
        {
            await using var conn = await OpenConnectionAsync();
            await using var batch = new NpgsqlBatch
            {
                Connection = conn,
                BatchCommands =
                {
                    new NpgsqlBatchCommand("SELECT @p")
                    {
                        Parameters = { new NpgsqlParameter("p", 8) }
                    },
                    new NpgsqlBatchCommand("SELECT @p1, @p2")
                    {
                        Parameters =
                        {
                            new NpgsqlParameter("p1", 9),
                            new NpgsqlParameter("p2", 10)
                        }
                    },
                }
            };

            await using var reader = await batch.ExecuteReaderAsync();
            Assert.That(await reader.ReadAsync(), Is.True);
            Assert.That(reader.FieldCount, Is.EqualTo(1));
            Assert.That(reader[0], Is.EqualTo(8));
            Assert.That(await reader.ReadAsync(), Is.False);
            Assert.That(await reader.NextResultAsync(), Is.True);
            Assert.That(reader.FieldCount, Is.EqualTo(2));
            Assert.That(await reader.ReadAsync(), Is.True);
            Assert.That(reader[0], Is.EqualTo(9));
            Assert.That(reader[1], Is.EqualTo(10));
            Assert.That(await reader.ReadAsync(), Is.False);
            Assert.That(await reader.NextResultAsync(), Is.False);
        }

        [Test]
        public async Task Positional_parameters()
        {
            await using var conn = await OpenConnectionAsync();
            await using var batch = new NpgsqlBatch
            {
                Connection = conn,
                BatchCommands =
                {
                    new NpgsqlBatchCommand("SELECT $1")
                    {
                        Parameters = { new NpgsqlParameter { Value = 8 } }
                    },
                    new NpgsqlBatchCommand("SELECT $1, $2")
                    {
                        Parameters = {
                            new NpgsqlParameter { Value = 9 },
                            new NpgsqlParameter { Value = 10 }
                        }
                    },
                }
            };

            await using var reader = await batch.ExecuteReaderAsync();
            Assert.That(await reader.ReadAsync(), Is.True);
            Assert.That(reader.FieldCount, Is.EqualTo(1));
            Assert.That(reader[0], Is.EqualTo(8));
            Assert.That(await reader.ReadAsync(), Is.False);
            Assert.That(await reader.NextResultAsync(), Is.True);
            Assert.That(await reader.ReadAsync(), Is.True);
            Assert.That(reader.FieldCount, Is.EqualTo(2));
            Assert.That(reader[0], Is.EqualTo(9));
            Assert.That(reader[1], Is.EqualTo(10));
            Assert.That(await reader.ReadAsync(), Is.False);
            Assert.That(await reader.NextResultAsync(), Is.False);
        }

        [Test]
        public async Task Single_batch_command()
        {
            await using var conn = await OpenConnectionAsync();
            await using var batch = new NpgsqlBatch
            {
                Connection = conn,
                BatchCommands = { new NpgsqlBatchCommand("SELECT 8") }
            };

            await using var reader = await batch.ExecuteReaderAsync();
            Assert.That(await reader.ReadAsync(), Is.True);
            Assert.That(reader.FieldCount, Is.EqualTo(1));
            Assert.That(reader[0], Is.EqualTo(8));
            Assert.That(await reader.ReadAsync(), Is.False);
            Assert.That(await reader.NextResultAsync(), Is.False);
        }

        [Test]
        public async Task Empty_batch()
        {
            await using var conn = await OpenConnectionAsync();
            await using var batch = new NpgsqlBatch { Connection = conn };
            await using var reader = await batch.ExecuteReaderAsync();

            Assert.That(await reader.ReadAsync(), Is.False);
            Assert.That(await reader.NextResultAsync(), Is.False);
        }

        // TODO: Preparation (explicit, auto).

        [Test]
        public async Task SingleRow()
        {
            await using var conn = await OpenConnectionAsync();
            await using var batch = new NpgsqlBatch
            {
                Connection = conn,
                BatchCommands =
                {
                    new NpgsqlBatchCommand("SELECT generate_series(1, 5)"),
                    new NpgsqlBatchCommand("SELECT generate_series(1, 5)") { CommandBehavior = CommandBehavior.SingleRow },
                    new NpgsqlBatchCommand("SELECT generate_series(1, 5)")
                }
            };

            await using var reader = await batch.ExecuteReaderAsync();

            for (var i = 0; i < 5; i++)
                Assert.That(await reader.ReadAsync(), Is.True);
            Assert.That(await reader.ReadAsync(), Is.False);

            Assert.That(await reader.ReadAsync(), Is.True);
            Assert.That(await reader.ReadAsync(), Is.False);

            for (var i = 0; i < 5; i++)
                Assert.That(await reader.ReadAsync(), Is.True);
            Assert.That(await reader.ReadAsync(), Is.False);
        }

        [Test]
        public async Task Semicolon_is_not_allowed()
        {
            await using var conn = await OpenConnectionAsync();
            await using var batch = new NpgsqlBatch
            {
                Connection = conn,
                BatchCommands = { new NpgsqlBatchCommand("SELECT 1; SELECT 2") }
            };

            Assert.That(() => batch.ExecuteReaderAsync(), Throws.Exception.TypeOf<NotSupportedException>());
        }

        [Test]
        public async Task Out_parameters_are_not_allowed()
        {
            await using var conn = await OpenConnectionAsync();
            await using var batch = new NpgsqlBatch
            {
                Connection = conn,
                BatchCommands =
                {
                    new NpgsqlBatchCommand("SELECT @p1")
                    {
                        Parameters = { new NpgsqlParameter("p", 8) { Direction = ParameterDirection.InputOutput} }
                    }
                }
            };

            Assert.That(() => batch.ExecuteReaderAsync(), Throws.Exception.TypeOf<NotSupportedException>());
        }

        public NpgsqlBatchTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
