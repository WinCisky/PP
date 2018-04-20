using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test02 : MonoBehaviour {

    public class Pollo
    {
        public int hp, atk;
        public string name;
        public Pollo()
        {
            hp = 3;
            atk = 1;
            name = "dummy";
        }
        public Pollo(int _hp, int _atk, string _name)
        {
            hp = _hp;
            atk = _atk;
            name = _name;
        }
    }

    public class Tile
    {
        public int x, y;
        public Pollo p;
        public Tile n, s, e, w;
        public GameObject t_go, p_go;
        public Tile()
        {
            x = 0;
            y = 0;
            p = null;
            n = null;
            s = null;
            e = null;
            w = null;
            t_go = null;
            p_go = null;
        }
        public Tile(int _x, int _y, GameObject _g)
        {
            x = _x;
            y = _y;
            p = null;
            n = null;
            s = null;
            e = null;
            w = null;
            p_go = null;
            t_go = _g;
            t_go.transform.position = new Vector3(x, y, 0);
        }
        public Tile(int _x, int _y, Pollo _p)
        {
            x = _x;
            y = _y;
            p = _p;
            n = null;
            s = null;
            e = null;
            w = null;
            t_go = null;
            p_go = null;
        }
        public Tile(int _x, int _y, Pollo _p, Tile _n, Tile _s, Tile _e, Tile _w)
        {
            x = _x;
            y = _y;
            p = _p;
            n = _n;
            s = _s;
            e = _e;
            w = _w;
            t_go = null;
            p_go = null;
        }
        public void Clean()
        {
            p = null;
        }
    }

    Tile[,] map;
    int map_size = 7;
    GameObject[] shown_tiles, shown_characters;
    public GameObject shown_tile, shown_character;
    public Sprite[] tile_sprites, character_sprites;
    public List<Pollo> player, enemy;

    public int player_count, enemy_count;

    public void InstantiateGO(GameObject[] g_o_list, int list_size, GameObject g, bool setUnactive)
    {
        int i = 0;
        while (i < list_size)
        {
            g_o_list[i++] = Instantiate(g);
            if (setUnactive)
                g_o_list[i - 1].SetActive(false);
        }            
    }

    public GameObject GetUnactiveGO(GameObject[] g_o_list)
    {
        foreach (var item in g_o_list)
        {
            if (!item.activeInHierarchy)
            {
                item.SetActive(true);
                return item;
            }
        }
        return null;
    }

    public void CleanMap()
    {
        //collegamenti fra le stanze
        for (int i = 0; i < map_size; i++)
        {
            for (int j = 0; j < map_size; j++)
            {
                if (i > 0)
                    map[i, j].w = map[i - 1, j];
                if (i < map_size - 1)
                    map[i, j].e = map[i + 1, j];
                if (j > 0)
                    map[i, j].s = map[i, j - 1];
                if (j < map_size - 1)
                    map[i, j].n = map[i, j + 1];
                map[i, j].Clean();
            }
        }
    }

    public void EnviromentSetup()
    {
        //creo dei muri in mezzo alla stanza
        //devo fare in modo che ci sia sempre un percorso da una parte all'altra della stanza
        //soluzione 1:
        //  uso map_size-1 muri al massimo
        for (int i = 0; i < (map_size-1); i++)
        {
            int x = Random.Range(1, map_size - 1);
            int y = Random.Range(1, map_size - 1);
            int direction = Random.Range(0, 3);
            switch (direction)
            {
                case 0:
                    map[x, y].n = null;
                    map[x, y + 1].s = null;
                    break;
                case 1:
                    map[x, y].s = null;
                    map[x, y - 1].n = null;
                    break;
                case 2:
                    map[x, y].e = null;
                    map[x + 1, y].w = null;
                    break;
                case 3:
                    map[x, y].w = null;
                    map[x - 1, y].e = null;
                    break;
            }
        }
    }

    void CharactersSetup(int p_count, int e_count)
    {
        switch (p_count)
        {
            case 1:
                map[3, 0].p_go = GetUnactiveGO(shown_characters);
                map[3, 0].p = player[0];
                break;
            case 2:
                map[3, 0].p_go = GetUnactiveGO(shown_characters);
                map[3, 0].p = player[0];
                map[4, 0].p_go = GetUnactiveGO(shown_characters);
                map[4, 0].p = player[1];
                break;
            case 3:
                map[3, 0].p_go = GetUnactiveGO(shown_characters);
                map[3, 0].p = player[0];
                map[4, 0].p_go = GetUnactiveGO(shown_characters);
                map[4, 0].p = player[1];
                map[2, 0].p_go = GetUnactiveGO(shown_characters);
                map[2, 0].p = player[2];
                break;
        }
        switch (e_count)
        {
            case 1:
                map[3, 6].p_go = GetUnactiveGO(shown_characters);
                map[3, 6].p = enemy[0];
                break;
            case 2:
                map[3, 6].p_go = GetUnactiveGO(shown_characters);
                map[3, 6].p = enemy[0];
                map[2, 6].p_go = GetUnactiveGO(shown_characters);
                map[2, 6].p = enemy[1];
                break;
            case 3:
                map[3, 6].p_go = GetUnactiveGO(shown_characters);
                map[3, 6].p = enemy[0];
                map[2, 6].p_go = GetUnactiveGO(shown_characters);
                map[2, 6].p = enemy[1];
                map[4, 6].p_go = GetUnactiveGO(shown_characters);
                map[4, 6].p = enemy[2];
                break;
        }
    }

    public Sprite GetTileSprite(Tile c)
    {
        if (c.n != null)
        {
            if (c.w != null)
            {
                if (c.e != null)
                {
                    if (c.s != null)
                    {
                        return tile_sprites[0];
                    }
                    else
                    {
                        return tile_sprites[3];
                    }
                }
                else
                {
                    if (c.s != null)
                    {
                        return tile_sprites[2];
                    }
                    else
                    {
                        return tile_sprites[6];
                    }
                }
            }
            else
            {
                if (c.e != null)
                {
                    if (c.s != null)
                    {
                        return tile_sprites[4];
                    }
                    else
                    {
                        return tile_sprites[7];
                    }
                }
                else
                {
                    if (c.s != null)
                    {
                        return tile_sprites[10];
                    }
                    else
                    {
                        return tile_sprites[13];
                    }
                }
            }
        }
        else
        {
            if (c.w != null)
            {
                if (c.e != null)
                {
                    if (c.s != null)
                    {
                        return tile_sprites[1];
                    }
                    else
                    {
                        return tile_sprites[9];
                    }
                }
                else
                {
                    if (c.s != null)
                    {
                        return tile_sprites[5];
                    }
                    else
                    {
                        return tile_sprites[12];
                    }
                }
            }
            else
            {
                if (c.e != null)
                {
                    if (c.s != null)
                    {
                        return tile_sprites[8];
                    }
                    else
                    {
                        return tile_sprites[14];
                    }
                }
                else
                {
                    if (c.s != null)
                    {
                        return tile_sprites[11];
                    }
                    else
                    {
                        //errore
                        return tile_sprites[0];
                    }
                }
            }
        }
    }

    public void UpdateShownMap()
    {
        for (int i = 0; i < map_size; i++)
        {
            for (int j = 0; j < map_size; j++)
            {
                if (map[i, j].p != null)
                {
                    Debug.Log("here: " + i + " " + j);
                    map[i, j].p_go.transform.position = new Vector3(i, j, -1);
                }                    
                map[i, j].t_go.GetComponent<SpriteRenderer>().sprite = GetTileSprite(map[i, j]);

            }
        }
    }

    public void MovePlayerOnMap(char dir, Tile t)
    {
        //c'è un personaggio nella tile?
        //c'è la tile di destinazione
        //la tile di destinazione è già occupata?
        switch (dir)
        {
            case 'n':
                break;
            case 's':
                break;
            case 'e':
                break;
            case 'w':
                break;
        }
    }

    // Use this for initialization
    void Start () {
        player = new List<Pollo>();
        enemy = new List<Pollo>();
        //devo prendere i parametri dall'attività chiamante
        player_count = 1;
        player.Add(new Pollo());
        //devo prendere i parametri dall'attività chiamante
        enemy_count = 1;
        enemy.Add(new Pollo());
        //inizializzo la mappa
        shown_tiles = new GameObject[map_size * map_size];
        InstantiateGO(shown_tiles, map_size * map_size, shown_tile, false);
        shown_characters = new GameObject[player_count + enemy_count];
        InstantiateGO(shown_characters, player_count + enemy_count, shown_character, true);
        foreach (var item in shown_characters)
        {
            item.GetComponent<SpriteRenderer>().sprite = character_sprites[0];
        }

        map = new Tile[map_size, map_size];
        for (int i = 0; i < map_size; i++)
        {
            for (int j = 0; j < map_size; j++)
            {
                map[i, j] = new Tile(i, j, shown_tiles[(i * map_size) + j]);
            }
        }
        CleanMap();
        EnviromentSetup();
        CharactersSetup(player_count, enemy_count);
        UpdateShownMap();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.W))
            print("up");
        if (Input.GetKeyDown(KeyCode.S))
            print("down");
        if (Input.GetKeyDown(KeyCode.D))
            print("right");
        if (Input.GetKeyDown(KeyCode.A))
            print("left");
    }
}
