using System.Collections.Concurrent;
using System.Diagnostics;
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
            var field = FourLayers.CreateOrderedField(fieldWidth);
            FourLayers.PrintField(field, useColors: false);
            var disorder = FourLayers.CalcDisorder(field, field, fieldWidth);
            Console.WriteLine(disorder);
            Assert.That(disorder, Is.EqualTo(0));
        }

        [Test]
        public void Test_CalcDisorder()
        {
            const byte fieldWidth = 8;
            var orderedFieldLookup = FourLayers.CreateOrderedField(fieldWidth);
            var field = (byte[,])orderedFieldLookup.Clone();
            FourLayers.PrintField(field, useColors: false);
            var disorder = FourLayers.CalcDisorder(field, orderedFieldLookup, fieldWidth);
            Console.WriteLine(disorder);
            Assert.That(disorder, Is.EqualTo(0));

            // Move 0 => IsUnchanged => Still Disorder 0 
            var moveResult = FourLayers.ProcessMove(field, fieldWidth, orderedFieldLookup, 0);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            disorder = FourLayers.CalcDisorder(moveResult.NewField, orderedFieldLookup, fieldWidth);
            Console.WriteLine(disorder);
            Assert.That(disorder, Is.EqualTo(0));
            Assert.That(moveResult.IsUnchanged, Is.EqualTo(true));

            // Move 1 => Disorder 2
            moveResult = FourLayers.ProcessMove(field, fieldWidth, orderedFieldLookup, 1);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            disorder = FourLayers.CalcDisorder(moveResult.NewField, orderedFieldLookup, fieldWidth);
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
            var ordereFieldlookup = FourLayers.CreateOrderedField(fieldWidth);
            var field = FourLayers.CreateOrderedField(fieldWidth);

            FourLayers.PrintField(field, useColors: false);
            var moveResult = FourLayers.ProcessMove(field, fieldWidth, ordereFieldlookup, 3);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            var disorder = FourLayers.CalcDisorder(moveResult.NewField, ordereFieldlookup, fieldWidth);
            Console.WriteLine(disorder);

            moveResult = FourLayers.ProcessMove(moveResult.NewField, fieldWidth, ordereFieldlookup, 4);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            disorder = FourLayers.CalcDisorder(moveResult.NewField, ordereFieldlookup, fieldWidth);
            Console.WriteLine(disorder);
        }

        [Test]
        public void Test_DeserialzeField()
        {
            var str = @"
  1 1 1 1
aaa
1  0  - 0 1
1 0 , 0 1
1 11 ; 1 ;       

";
            var field = FourLayers.DeserializeField(str);
            FourLayers.PrintField(field.Field, useColors: false);
            var lookup = FourLayers.CreateOrderedField(field.FieldWidth);
            var disorder = FourLayers.CalcDisorder(field.Field, lookup, field.FieldWidth);
            Assert.That(disorder, Is.EqualTo(0));
        }

    }
}
