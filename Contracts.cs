using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Patterns.Contracts;
using PostSharp.Reflection;

namespace Game1 {
    public class NonEmptyAttribute : LocationContractAttribute,
        ILocationValidationAspect<Point>,
        ILocationValidationAspect<Rectangle> {
        public Exception ValidateValue(Point value, string locationName, LocationKind locationKind, LocationValidationContext context) {
            if (value == Point.Zero)
                return new ArgumentOutOfRangeException($"The coordinates for {locationName} must not be (0, 0).");
            else
                return null;
        }

        public Exception ValidateValue(Rectangle value, string locationName, LocationKind locationKind, LocationValidationContext context) {
            if (value.Size == Point.Zero)
                return new ArgumentOutOfRangeException($"The size of {value} must not be 0.");
            else
                return null;
        }
    }
}
