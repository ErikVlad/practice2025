using System;

namespace task04;

public class Cruiser : ISpaceship
{
    public int Speed
    {
        get { return 50; }
    }

    public int FirePower
    {
        get { return 100; }
    }

    public void MoveForward()
    {
    }

    public void Rotate(int angle)
    {
    }

    public void Fire()
    {
    }
}
