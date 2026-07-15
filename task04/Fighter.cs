using System;

namespace task04;

public class Fighter : ISpaceship
{
    public int Speed
    {
        get { return 100; }
    }

    public int FirePower
    {
        get { return 20; }
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
