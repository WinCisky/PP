using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class Character
{
    public string name;
    public int atk, def, hp;
    public Character()
    {
        name = "Dummy";
        atk = 1;
        def = 0;
        hp = 5;
    }
    public Character(int _atk, int _def, int _hp)
    {
        name = "Dummy";
        atk = _atk;
        def = _def;
        hp = _hp;
    }
    public Character(string _name, int _atk, int _def, int _hp)
    {
        name = _name;
        atk = _atk;
        def = _def;
        hp = _hp;
    }
}

public class Group
{
    //è il gruppo del giocatore?
    public bool player;
    public GameObject shown_GO;
    public List<Character> members;
    public Group()
    {
        members = new List<Character>();
        player = false;
        shown_GO = null;
    }
}

public class Cell
{
    public int x, y;
    public Group defender, striker;
    public GameObject shown_GO;
    public Cell up, down, left, right;
    public Cell()
    {
        x = 0;
        y = 0;
        defender = null;
        striker = null;
        shown_GO = null;
        up = null;
        down = null;
        left = null;
        right = null;
    }
    public Cell(int _x, int _y)
    {
        x = _x;
        y = _y;
        defender = null;
        striker = null;
        shown_GO = null;
        up = null;
        down = null;
        left = null;
        right = null;
    }
}

public class LevelExpansion
{
    public int x, y;
    public Cell source_cell;
    public LevelExpansion(int _x, int _y, Cell c)
    {
        x = _x;
        y = _y;
        source_cell = c;
    }
}

public class Test01 : MonoBehaviour {

    //pool di personaggi
    public List<GameObject> enemies;
    //pool di caselle
    public List<GameObject> cells;
    //livello corrente
    public List<Cell> level;
    //oggetto usato per mostrare le celle
    public GameObject cell_GO;
    //spites per le caselle
    public List<Sprite> shown_GO_sprites;
    //oggetto usato per mostrare i nemici
    public GameObject enemy_GO;
    //spites per i nemici
    public List<Sprite> enemies_GO_sprites;
    //lista dei nemici nel livello
    public List<Cell> enemies_on_level;
    //gioco fermo?
    public bool stopped;

    /* POOL MANAGEMENT START */

    public void Create_GO_Pool(List<GameObject> l, GameObject g)
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject obj = Instantiate(g);
            obj.SetActive(false);
            l.Add(obj);
        }
    }

    public GameObject Get_GO_FromPool(List<GameObject> l)
    {
        //cerco un oggetto non attivo
        foreach (var item in l)
        {
            if (!item.activeInHierarchy)
                return item;
        }
        //se non lo trovo lo creo
        GameObject obj = Instantiate(cell_GO);
        obj.SetActive(false);
        l.Add(obj);
        return obj;
    }

    public void Reset_GO_Pool(List<GameObject> l)
    {
        foreach (var item in l)
        {
            item.SetActive(false);
        }
    }

    /* POOL MANAGEMENT END */
    /* MAP MANAGEMENT START */

    public Cell CellAt(int _x, int _y, List<Cell> l)
    {
        foreach (var item in l)
        {
            if (item.x == _x && item.y == _y)
                return item;
        }
        return null;
    }

    public char CreateConnection(Cell c1, Cell c2)
    {
        int dir_x = c1.x - c2.x;
        int dir_y = c1.y - c2.y;
        //valore di ritorno inizializzato a nord
        char return_value = 'n';
        switch (dir_x)
        {
            case 0:
                switch (dir_y)
                {
                    case 1:
                        return_value = 'n';
                        c1.down = c2;
                        c2.up = c1;
                        break;
                    case -1:
                        return_value = 's';
                        c1.up = c2;
                        c2.down = c1;
                        break;
                }
                break;
            case 1:
                return_value = 'e';
                c1.left = c2;
                c2.right = c1;
                break;
            case -1:
                return_value = 'w';
                c1.right = c2;
                c2.left = c1;
                break;
        }
        return return_value;
    }

    public void AddPossibleExpansions(List<LevelExpansion> l, char dir, Cell c)
    {
        switch (dir)
        {
            case 'n':
                l.Add(new LevelExpansion(c.x, c.y + 1, c));
                l.Add(new LevelExpansion(c.x + 1, c.y, c));
                l.Add(new LevelExpansion(c.x - 1, c.y, c));
                break;
            case 's':
                l.Add(new LevelExpansion(c.x, c.y - 1, c));
                l.Add(new LevelExpansion(c.x + 1, c.y, c));
                l.Add(new LevelExpansion(c.x - 1, c.y, c));
                break;
            case 'e':
                l.Add(new LevelExpansion(c.x + 1, c.y, c));
                l.Add(new LevelExpansion(c.x, c.y + 1, c));
                l.Add(new LevelExpansion(c.x, c.y - 1, c));
                break;
            case 'w':
                l.Add(new LevelExpansion(c.x - 1, c.y, c));
                l.Add(new LevelExpansion(c.x, c.y + 1, c));
                l.Add(new LevelExpansion(c.x, c.y - 1, c));
                break;
        }
    }

    public Cell AddCellToMap(List<LevelExpansion> expansions, List<Cell> level, int i, int max)
    {
        //ho troato una cella da aggiungere al livello?
        bool found = false;
        //la nuova cella che verrà restituita
        Cell new_cell = new Cell();
        //c'è almeno una espansione disponibile
        //tento di aggiungere una casella fino a quando questa non viene effettivamente aggiunta
        while (!found)
        {
            int random = Random.Range(0, expansions.Count);
            if (CellAt(expansions[random].x, expansions[random].y, level) == null)
            {
                found = true;
                new_cell.x = expansions[random].x;
                new_cell.y = expansions[random].y;
                //creo il collegamento tra la cella di provenienza e la nuova cella
                char direction = CreateConnection(new_cell, expansions[random].source_cell);
                //tolgo l'espansione creata
                expansions.Remove(expansions[random]);
                //aggiungo le nuove espansioni possibili alla lista delle espansioni
                AddPossibleExpansions(expansions, direction, new_cell);
            }
        }
        //CREAZIONE NEMICI
        if (max - i < 5)
        {
            //creo il nemico
            Group enemyGroup = new Group();
            enemyGroup.members.Add(new Character(1, 0, 3));
            //lo aggiungo alla lista dei nemici
            enemies_on_level.Add(new_cell);
            new_cell.defender = enemyGroup;
        }
        return new_cell;
    }

    public void CreateLevel(int cell_number, int level_number)
    {
        //lista di tutte le celle
        level = new List<Cell>();
        //lista delle espansioni discponibili
        List<LevelExpansion> expandable_cells = new List<LevelExpansion>();
        //creo la cella centrale
        Cell center = new Cell(0, 0);
        level.Add(center);
        //aggiungo le quattro espansioni disponibili dalla cella centrale
        expandable_cells.Add(new LevelExpansion(1, 0, center));
        expandable_cells.Add(new LevelExpansion(-1, 0, center));
        expandable_cells.Add(new LevelExpansion(0, 1, center));
        expandable_cells.Add(new LevelExpansion(0, -1, center));
        for (int i = 0; i < cell_number - 1; i++)
        {
            level.Add(AddCellToMap(expandable_cells, level, i, cell_number));
        }
    }

    public Sprite GetCellSprite(Cell c)
    {
        if (c.up != null)
        {
            if (c.left != null)
            {
                if (c.right != null)
                {
                    if (c.down != null)
                    {
                        return shown_GO_sprites[0];
                    }
                    else
                    {
                        return shown_GO_sprites[3];
                    }
                }
                else
                {
                    if (c.down != null)
                    {
                        return shown_GO_sprites[2];
                    }
                    else
                    {
                        return shown_GO_sprites[6];
                    }
                }
            }
            else
            {
                if (c.right != null)
                {
                    if (c.down != null)
                    {
                        return shown_GO_sprites[4];
                    }
                    else
                    {
                        return shown_GO_sprites[7];
                    }
                }
                else
                {
                    if (c.down != null)
                    {
                        return shown_GO_sprites[10];
                    }
                    else
                    {
                        return shown_GO_sprites[13];
                    }
                }
            }
        }
        else
        {
            if (c.left != null)
            {
                if (c.right != null)
                {
                    if (c.down != null)
                    {
                        return shown_GO_sprites[1];
                    }
                    else
                    {
                        return shown_GO_sprites[9];
                    }
                }
                else
                {
                    if (c.down != null)
                    {
                        return shown_GO_sprites[5];
                    }
                    else
                    {
                        return shown_GO_sprites[12];
                    }
                }
            }
            else
            {
                if (c.right != null)
                {
                    if (c.down != null)
                    {
                        return shown_GO_sprites[8];
                    }
                    else
                    {
                        return shown_GO_sprites[14];
                    }
                }
                else
                {
                    if (c.down != null)
                    {
                        return shown_GO_sprites[11];
                    }
                    else
                    {
                        //errore
                        return shown_GO_sprites[0];
                    }
                }
            }
        }
    }

    public Sprite GetEnemySprite(Group g, bool defender)
    {
        if (defender)
        {
            switch (g.members.Count)
            {
                case 0:
                    Debug.Log("error! empty group");
                    return null;
                case 1:
                    return enemies_GO_sprites[10];
                case 2:
                    return enemies_GO_sprites[11];
                case 3:
                    return enemies_GO_sprites[12];
                case 4:
                    return enemies_GO_sprites[13];
                default: //5+
                    return enemies_GO_sprites[14];
            }
        }
        else //attaccante
        {
            switch (g.members.Count)
            {
                case 0:
                    Debug.Log("error! empty group");
                    return null;
                case 1:
                    return enemies_GO_sprites[15];
                case 2:
                    return enemies_GO_sprites[16];
                case 3:
                    return enemies_GO_sprites[17];
                case 4:
                    return enemies_GO_sprites[18];
                default: //5+
                    return enemies_GO_sprites[19];
            }
        }
    }

    public Sprite GetPlayerSprite(Group g, bool defender)
    {
        if (defender)
        {
            switch (g.members.Count)
            {
                case 0:
                    Debug.Log("error! empty group");
                    return null;
                case 1:
                    return enemies_GO_sprites[0];
                case 2:
                    return enemies_GO_sprites[1];
                case 3:
                    return enemies_GO_sprites[2];
                case 4:
                    return enemies_GO_sprites[3];
                default: //5+
                    return enemies_GO_sprites[4];
            }
        }
        else //attaccante
        {
            switch (g.members.Count)
            {
                case 0:
                    Debug.Log("error! empty group");
                    return null;
                case 1:
                    return enemies_GO_sprites[5];
                case 2:
                    return enemies_GO_sprites[6];
                case 3:
                    return enemies_GO_sprites[7];
                case 4:
                    return enemies_GO_sprites[8];
                default: //5+
                    return enemies_GO_sprites[9];
            }
        }
    }

    public void SetEnemySprite(Cell c)
    {
        c.defender.shown_GO = Get_GO_FromPool(enemies);
        c.defender.shown_GO.transform.position = new Vector3(c.x, c.y);

        if (c.defender != null)
        {
            if (c.defender.player)
            {
                //giocatore def
                c.defender.shown_GO = Get_GO_FromPool(enemies);
                c.defender.shown_GO.SetActive(true);
                c.defender.shown_GO.GetComponent<SpriteRenderer>().sprite = GetPlayerSprite(c.defender, true);
            }
            else
            {
                //npc def
                c.defender.shown_GO = Get_GO_FromPool(enemies);
                c.defender.shown_GO.SetActive(true);
                c.defender.shown_GO.GetComponent<SpriteRenderer>().sprite = GetEnemySprite(c.defender, true);
            }
            if (c.striker != null)
            {
                if (c.striker.player)
                {
                    //giocatore att
                    c.striker.shown_GO = Get_GO_FromPool(enemies);
                    c.striker.shown_GO.SetActive(true);
                    c.striker.shown_GO.GetComponent<SpriteRenderer>().sprite = GetPlayerSprite(c.defender, false);
                }
                else
                {
                    //npc att
                    c.striker.shown_GO = Get_GO_FromPool(enemies);
                    c.striker.shown_GO.SetActive(true);
                    c.striker.shown_GO.GetComponent<SpriteRenderer>().sprite = GetEnemySprite(c.striker, false);
                }
            }
        }
    }

    public Color AlbedoFade(Color c, int remaining_steps)
    {
        Color result = c;
        switch (remaining_steps)
        {
            default:
                result.a = 1;
                break;
            case 9:
                result.a = .9f;
                break;
            case 8:
                result.a = .8f;
                break;
            case 7:
                result.a = .7f;
                break;
            case 6:
                result.a = .6f;
                break;
            case 5:
                result.a = .5f;
                break;
            case 4:
                result.a = .4f;
                break;
            case 3:
                result.a = .3f;
                break;
            case 2:
                result.a = .2f;
                break;
            case 1:
                result.a = .1f;
                break;
        }
        return result;
    }

    public void ShowAisle(Cell c, int remaining_steps, char dir)
    {
        if(remaining_steps > 0)
        {
            if (c != null)
            {
                c.shown_GO = Get_GO_FromPool(cells);
                c.shown_GO.SetActive(true);
                c.shown_GO.transform.position = new Vector3(c.x, c.y);
                c.shown_GO.GetComponent<SpriteRenderer>().sprite = GetCellSprite(c);
                //controllo se c'è un nemico sulla casella
                if (c.defender != null)
                {
                    //mostro il nemico
                    SetEnemySprite(c);
                }
                //sfumo alle estremità
                c.shown_GO.GetComponent<SpriteRenderer>().color = AlbedoFade(c.shown_GO.GetComponent<SpriteRenderer>().color, remaining_steps);
                //mostro la cella e faccio la ricorsione
                switch (dir)
                {
                    case 'n':
                        ShowAisle(c.up, remaining_steps - 1, 'n');
                        ShowAisle(c.right, remaining_steps - 1, 'e');
                        ShowAisle(c.left, remaining_steps - 1, 'w');
                        break;
                    case 's':
                        ShowAisle(c.down, remaining_steps - 1, 's');
                        ShowAisle(c.right, remaining_steps - 1, 'e');
                        ShowAisle(c.left, remaining_steps - 1, 'w');
                        break;
                    case 'e':
                        ShowAisle(c.up, remaining_steps - 1, 'n');
                        ShowAisle(c.down, remaining_steps - 1, 's');
                        ShowAisle(c.right, remaining_steps - 1, 'e');
                        break;
                    case 'w':
                        ShowAisle(c.up, remaining_steps - 1, 'n');
                        ShowAisle(c.down, remaining_steps - 1, 's');
                        ShowAisle(c.left, remaining_steps - 1, 'w');
                        break;
                }
            }
        }
    }

    public void ShowMap(Cell player_pos)
    {
        /*
        foreach (var item in level)
        {
            item.shown_GO = Instantiate(cell_GO);
            item.shown_GO.transform.position = new Vector3(item.x, item.y);
            item.shown_GO.GetComponent<SpriteRenderer>().sprite = GetCellSprite(item);
        }
        */
        int distance = 6;
        Reset_GO_Pool(cells);
        Reset_GO_Pool(enemies);
        player_pos.shown_GO = Get_GO_FromPool(cells);
        player_pos.shown_GO.SetActive(true);
        player_pos.shown_GO.transform.position = new Vector3(player_pos.x, player_pos.y);
        player_pos.shown_GO.GetComponent<SpriteRenderer>().sprite = GetCellSprite(player_pos);
        //mostro il giocatore
        SetEnemySprite(player_pos);
        player_pos.shown_GO.GetComponent<SpriteRenderer>().color = AlbedoFade(player_pos.shown_GO.GetComponent<SpriteRenderer>().color, distance);
        ShowAisle(player_pos.up, distance, 'n');
        ShowAisle(player_pos.down, distance, 's');
        ShowAisle(player_pos.right, distance, 'e');
        ShowAisle(player_pos.left, distance, 'w');
    }

    /* MAP MANAGEMENT END */
    /* CHARACTERS MANAGEMENT START */

    public Cell MoveGroup(Cell group_cell, char direction)
    {
        Cell movement_result = group_cell;
        switch (direction)
        {
            case 'n':
                if (group_cell.striker == null)
                {
                    if (group_cell.up != null)
                    { 
                        if (group_cell.up.defender != null)
                        {
                            if (group_cell.up.striker != null)
                            {
                                Debug.Log("can't join fight!");
                            }
                            else
                            {
                                if(group_cell.defender.player || group_cell.up.defender.player)
                                {
                                    group_cell.up.striker = group_cell.defender;
                                    movement_result = group_cell.up;
                                    group_cell.defender = null;
                                }
                            }
                        }
                        else
                        {
                            group_cell.up.defender = group_cell.defender;
                            movement_result = group_cell.up;
                            group_cell.defender = null;
                        }
                    }
                }
                else
                {
                    Debug.Log("in battle, can't move!");
                }                
                break;
            case 's':
                if (group_cell.striker == null)
                {
                    if (group_cell.down != null)
                    {
                        if (group_cell.down.defender != null)
                        {
                            if (group_cell.down.striker != null)
                            {
                                Debug.Log("can't join fight!");
                            }
                            else
                            {
                                if (group_cell.defender.player || group_cell.down.defender.player)
                                {
                                    group_cell.down.striker = group_cell.defender;
                                    movement_result = group_cell.down;
                                    group_cell.defender = null;
                                }
                            }
                        }
                        else
                        {
                            group_cell.down.defender = group_cell.defender;
                            movement_result = group_cell.down;
                            group_cell.defender = null;
                        }
                    }
                }
                else
                {
                    Debug.Log("in battle, can't move!");
                }
                break;
            case 'e':
                if (group_cell.striker == null)
                {
                    if (group_cell.right != null)
                    {
                        if (group_cell.right.defender != null)
                        {
                            if (group_cell.right.striker != null)
                            {
                                Debug.Log("can't join fight!");
                            }
                            else
                            {
                                if (group_cell.defender.player || group_cell.right.defender.player)
                                {
                                    group_cell.right.striker = group_cell.defender;
                                    movement_result = group_cell.right;
                                    group_cell.defender = null;
                                }
                            }
                        }
                        else
                        {
                            group_cell.right.defender = group_cell.defender;
                            movement_result = group_cell.right;
                            group_cell.defender = null;
                        }
                    }
                }
                else
                {
                    Debug.Log("in battle, can't move!");
                }
                break;
            case 'w':
                if (group_cell.striker == null)
                {
                    if (group_cell.left != null)
                    {
                        if (group_cell.left.defender != null)
                        {
                            if (group_cell.left.striker != null)
                            {
                                Debug.Log("can't join fight!");
                            }
                            else
                            {
                                if (group_cell.defender.player || group_cell.left.defender.player)
                                {
                                    group_cell.left.striker = group_cell.defender;
                                    movement_result = group_cell.left;
                                    group_cell.defender = null;
                                }
                            }
                        }
                        else
                        {
                            group_cell.left.defender = group_cell.defender;
                            movement_result = group_cell.left;
                            group_cell.defender = null;
                        }
                    }
                }
                else
                {
                    Debug.Log("in battle, can't move!");
                }
                break;
        }
        //creo il combattimento
        if (InFight(movement_result))
            START_FIGHT(movement_result);
        return movement_result;
    }

    public bool InFight(Cell c)
    {
        if (c.defender != null && c.striker != null)
            return true;
        return false;
    }

    public void START_FIGHT(Cell c)
    {
        bool ended = false;
        while (!ended)
        {
            ended = true;
        }
    }

    public void MoveEnemies()
    {
        List<Cell> new_enemy_positions = new List<Cell>();
        foreach (var group in enemies_on_level)
        {
            //muovo a caso
            switch (Random.Range(0, 4))
            {
                case 0:
                    new_enemy_positions.Add(MoveGroup(group, 'n'));
                    break;
                case 1:
                    new_enemy_positions.Add(MoveGroup(group, 's'));
                    break;
                case 2:
                    new_enemy_positions.Add(MoveGroup(group, 'e'));
                    break;
                case 3:
                    new_enemy_positions.Add(MoveGroup(group, 'w'));
                    break;
            }
        }
        enemies_on_level = new_enemy_positions;
    }

    public Cell MovePlayer(Cell player_group_cell, char direction)
    {
        //esegui il movimento
        player_group_cell = MoveGroup(player_group_cell, direction);
        //mossa nemici
        MoveEnemies();
        //aggiorna mappa
        ShowMap(player_group_cell);

        return player_group_cell;
    }

    /* CHARACTERS MANAGEMENT END */

    Cell player_cell;

    private void Start()
    {
        //TESTING PURPOSE
        stopped = false;
        cells = new List<GameObject>();
        enemies = new List<GameObject>();
        //Pool di caselle mostrate
        Create_GO_Pool(cells, cell_GO);
        //Pool di nemici mostrati
        Create_GO_Pool(enemies, enemy_GO);
        //inizializzo la lista dei nemici
        enemies_on_level = new List<Cell>();
        //Crea la logica del livello
        CreateLevel(500,0);
        //creo il gruppo del giocatorer
        Group player_group = new Group();
        //imposto che il gruppo è controllato dal giocatore
        player_group.player = true;
        //aggiungo il personaggio principale
        player_group.members.Add(new Character("hero", 1, 0, 5));
        //posiziono il giocatore al centro della mappa
        player_cell = CellAt(0, 0, level);
        //assegno il giocatore come difensore della casella centrale
        player_cell.defender = player_group;
        //mostro la mappa visivamente
        ShowMap(player_cell);
    }

    private void Update()
    {
        if (!stopped)
        {

            //input utente
            if (Input.GetKeyDown(KeyCode.W))
            {
                player_cell = MovePlayer(player_cell, 'n');
                Camera.main.gameObject.GetComponent<SmoothFollow>().target = player_cell.shown_GO.transform;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                player_cell = MovePlayer(player_cell, 's');
                Camera.main.gameObject.GetComponent<SmoothFollow>().target = player_cell.shown_GO.transform;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                player_cell = MovePlayer(player_cell, 'w');
                Camera.main.gameObject.GetComponent<SmoothFollow>().target = player_cell.shown_GO.transform;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                player_cell = MovePlayer(player_cell, 'e');
                Camera.main.gameObject.GetComponent<SmoothFollow>().target = player_cell.shown_GO.transform;
            }
        }
    }
}

/* TODO
 * 
 * Save Level
 * Load level
 * Remove some walls function
 * 
 * Group movement in level
 * 
 * Phone input
 *   selection / drag & drop
 *   direction movement
 *   selection update
 * Enemy movement in level
 *   automatic reaction to player movement
 *   choose a random direction to follow if possible
 * 
 */
