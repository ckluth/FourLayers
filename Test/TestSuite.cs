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
            var rating = FourLayers.GetRating(field, lookup, fieldWidth);
            Console.WriteLine(rating);
            Assert.That( rating, Is.EqualTo(0));
        }

        [Test]
        public void Test_GetRating()
        {
            const byte fieldWidth = 8;
            var lookup = FourLayers.CreateOrderedFieldLookUp(fieldWidth);
            var field = FourLayers.CreateOrderedField(lookup);
            FourLayers.PrintField(field, useColors: false);
            var rating = FourLayers.GetRating(field, lookup, fieldWidth);
            Console.WriteLine(rating);
            Assert.That(rating, Is.EqualTo(0));

            // Move 0 => IsUnchanged => Still Rating 0 
            var moveResult = FourLayers.ProcessMove(field, fieldWidth, lookup, 0);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            rating = FourLayers.GetRating(moveResult.NewField, lookup, fieldWidth);
            Console.WriteLine(rating);
            Assert.That(rating, Is.EqualTo(0));
            Assert.That(moveResult.IsUnchanged, Is.EqualTo(true)); 

            // Move 1 => Rating 2
            moveResult = FourLayers.ProcessMove(field, fieldWidth, lookup, 1);
            FourLayers.PrintField(moveResult.NewField, useColors: false);
            rating = FourLayers.GetRating(moveResult.NewField, lookup, fieldWidth);
            Console.WriteLine(rating);
            Assert.That(rating, Is.EqualTo(2));
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

    }
}
