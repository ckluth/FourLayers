using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using NUnit.Framework;

namespace FourLayers
{
    public class TestSuite
    {
        [Test]
        public void Test_CreateOrderedField()
        {
            const byte fieldWidth = 8;
            var lookup = FourLayers.CreateOrderedFieldLookUp(fieldWidth);
            var field = FourLayers.CreateOrderedField(lookup);
            FourLayers.PrintField(field, useColors: false);
            var disorder = FourLayers.CalcDisorder(field, lookup, fieldWidth);
            Console.WriteLine(disorder);
            Assert.That(disorder, Is.EqualTo(0));
        }

        [Test]
        public void Test_CalcDisorder()
        {
            const byte fieldWidth = 8;
            var lookup = FourLayers.CreateOrderedFieldLookUp(fieldWidth);
            var field = FourLayers.CreateOrderedField(lookup);
            FourLayers.PrintField(field, useColors: false);
            var disorder = FourLayers.CalcDisorder(field, lookup, fieldWidth);
            Console.WriteLine(disorder);
            Assert.That(disorder, Is.EqualTo(0));

            // Move 0 => IsUnchanged => Still Disorder 0 
            var moveResult = FourLayers.ProcessMove(field, fieldWidth, lookup, 0);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            disorder = FourLayers.CalcDisorder(moveResult.NewField, lookup, fieldWidth);
            Console.WriteLine(disorder);
            Assert.That(disorder, Is.EqualTo(0));
            Assert.That(moveResult.IsUnchanged, Is.EqualTo(true)); 

            // Move 1 => Disorder 2
            moveResult = FourLayers.ProcessMove(field, fieldWidth, lookup, 1);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            disorder = FourLayers.CalcDisorder(moveResult.NewField, lookup, fieldWidth);
            Console.WriteLine(disorder);
            Assert.That(disorder, Is.EqualTo(2));
            Assert.That(moveResult.IsUnchanged, Is.EqualTo(false));
            Assert.That(moveResult.WasAffectedLineInOrder, Is.EqualTo(true));
        }

        [Test]
        public void Test_CreateSlots()
        {
            for (var i = 1; i < 128; i++)
            {
                for (var ii = 2; ii <= 4; ii++)
                {
                    var kerne = i;
                    var fieldWidth = ii * 2;
                    var slots = Slot.CreateSlots((byte)fieldWidth, (byte)kerne).Slots;
                    var slotCount = Math.Min(kerne, fieldWidth * 4);
                    Assert.That(slots.Count, Is.EqualTo(slotCount));
                    Assert.That(slots.Sum(slot => slot.SlotSize), Is.EqualTo(fieldWidth * 4));
                }
            }
        }

        [Test, Explicit]
        public void Test_CheckDisorder()
        {
            const byte fieldWidth = 8;
            var lookup = FourLayers.CreateOrderedFieldLookUp(fieldWidth);
            var field = FourLayers.CreateOrderedField(lookup);

            FourLayers.PrintField(field, useColors: false);
            var moveResult = FourLayers.ProcessMove(field, fieldWidth, lookup, 3);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            var disorder = FourLayers.CalcDisorder(moveResult.NewField, lookup, fieldWidth);
            Console.WriteLine(disorder);

            moveResult = FourLayers.ProcessMove(moveResult.NewField, fieldWidth, lookup, 4);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            disorder = FourLayers.CalcDisorder(moveResult.NewField, lookup, fieldWidth);
            Console.WriteLine(disorder);
        }
    }
}
