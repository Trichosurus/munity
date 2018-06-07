using System;

namespace Weland {
     public static class World {
	public const short One = 1024;
	public static short FromDouble(double d) {
	    return (short) Math.Round(d * World.One);
	}

	public static double ToDouble(short w) {
	    return ((double) w / World.One);
	}

	public static double ToDouble(int i) {
	    return ((double) i / World.One);
	}
    }

    public static class Angle {
	const short AngularPrecision = 512;
	public static short FromDouble(double d) {
	    return (short) Math.Round(d * AngularPrecision / 360);
	}
	public static double ToDouble(short a) {
	    return ((double) a * 360 / AngularPrecision);
	}
    }
}
