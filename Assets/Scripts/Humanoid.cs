using UnityEngine;
public class Humanoid {

    public int x, y;
    public int hp;
    public string name;
    public GameObject g;

    public Humanoid()
    {
        x = 0;
        y = 0;
        hp = 1;
        name = "dummy";
        g = null;
    }

    public Humanoid(int _x, int _y)
    {
        x = _x;
        y = _y;
        hp = 1;
        name = "dummy";
        g = null;
    }

    public Humanoid(int _x, int _y, int _hp)
    {
        x = _x;
        y = _y;
        hp = _hp;
        name = "dummy";
        g = null;
    }

    public Humanoid(int _x, int _y, string _name)
    {
        x = _x;
        y = _y;
        hp = 1;
        name = _name;
        g = null;
    }

    public Humanoid(int _x, int _y, int _hp, string _name)
    {
        x = _x;
        y = _y;
        hp = _hp;
        name = _name;
        g = null;
    }

}
