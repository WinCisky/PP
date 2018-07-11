using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PP
{
    public string name;
    public int hp;
    public int atk;
    public int shield_cooldown = 0;
    public bool shield_next = false;
    public bool attack_next = false;
    public bool dodge_next = false;
    public int attack_pos = 0;
    public int dodge_pos = 0;
    public bool action_selected = false;
    public bool powerful_attack = false;
    public PP(string name, int hp, int atk)
    {
        this.name = name;
        this.hp = hp;
        this.atk = atk;
    }
    public void TurnFinished()
    {
        action_selected = false;
        shield_next = false;
        dodge_next = false;
        attack_next = false;
        if (shield_cooldown > 0)
            shield_cooldown--;
    }
    public void ShieldUp()
    {
        shield_next = true;
    }
    public void AttackNext(int pos)
    {
        attack_next = true;
        attack_pos = pos;
    }
    public void DodgeNext(int dir)
    {
        dodge_next = true;
        dodge_pos = dir;
    }
}

public class Test03_fight2 : MonoBehaviour {

    public PP[] you, enemies;

    //canvas
    public GameObject _back, _char, _action, _dir;
    //bottoni dei canvas
    public Button _charL, _charR, _charC, _dodgeL, _dodgeR, _attackL, _attackR, _shield;
    //indicatori di vita residua
    public Text[] _textsHp, _textsMove;
    //azioni del giocatore per il turno corrente
    int step = 0, sel_char = 0, sel_action = 0, sel_dir = 0;

	// Use this for initialization
	void Start () {

        you = new PP[3];
        enemies = new PP[3];

        you[0] = null;
        you[1] = new PP("player", 3, 1);
        you[2] = null;
        enemies[0] = null;
        enemies[1] = new PP("enemy", 3, 1);
        enemies[2] = null;

        UIShow();

    }

    void PrintPP(PP[] item)
    {
        for (int i = 0; i < item.Length; i++)
        {
            if (item[i] != null)
            {
                Debug.Log(item[i].name + " " + item[i].hp + " " + item[i].atk);
            }
        }
    }

    int CharAlive(PP[] arr)
    {
        int sum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != null)
                sum++;
        }
        return sum;
    }

    int CharToBeMoved(PP[] arr)
    {
        int sum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != null && !arr[i].action_selected)
                sum++;
        }
        return sum;
    }

    void UpdateShownHp()
    {
        int you_len = you.Length;
        for (int i = 0; i < you_len; i++)
        {
            if (you[i] != null)
            {
                _textsHp[i].enabled = true;
                _textsHp[i].text = you[i].hp.ToString();
            }
            else _textsHp[i].enabled = false;
        }
        for (int i = you_len; i < (you_len + enemies.Length); i++)
        {
            if (enemies[i - you_len] != null)
            {
                _textsHp[i].enabled = true;
                _textsHp[i].text = enemies[i - you_len].hp.ToString();
            }
            else _textsHp[i].enabled = false;
        }
    }

    void ShowMove()
    {
        //pulisco i testi precedenti
        for (int i = 0; i < _textsMove.Length; i++)
        {
            _textsMove[i].text = "";
        }
        Debug.Log("oky");
        int you_len = you.Length;
        for (int i = 0; i < you_len; i++)
        {
            if (you[i] != null)
            {
                Debug.Log("status: " + you[i].attack_next + " " + you[i].dodge_next + " " + you[i].shield_next);
                if (you[i].attack_next)
                {
                    _textsMove[i].text += "atk ";
                    switch (you[i].attack_pos)
                    {
                        case -1:
                            _textsMove[i].text += "left";
                            break;
                        case 0:
                            _textsMove[i].text += "front";
                            break;
                        case 1:
                            _textsMove[i].text += "right";
                            break;
                    }
                }
                else if (you[i].dodge_next)
                    _textsMove[i].text = "dodge";
                else
                    _textsMove[i].text = "shield";
            }
        }
        for (int i = you_len; i < (you_len + enemies.Length); i++)
        {
            if (enemies[i - you_len] != null)
            {
                if (enemies[i - you_len].attack_next)
                {
                    _textsMove[i].text += "atk ";
                    switch (enemies[i - you_len].attack_pos)
                    {
                        case -1:
                            _textsMove[i].text += "left";
                            break;
                        case 0:
                            _textsMove[i].text += "front";
                            break;
                        case 1:
                            _textsMove[i].text += "right";
                            break;
                    }
                }
                else if (enemies[i - you_len].dodge_next)
                    _textsMove[i].text = "dodge";
                else
                    _textsMove[i].text = "shield";
            }
        }
    }

    public void UIShow()
    {
        if (CharToBeMoved(you) > 0)
        {
            UpdateShownHp();
            step = 0;
            _back.gameObject.SetActive(false);
            _action.gameObject.SetActive(false);
            _dir.gameObject.SetActive(false);
            _char.gameObject.SetActive(true);
            _charC.interactable = true;
            _charL.interactable = true;
            _charR.interactable = true;
            if (you[0] == null || you[0].action_selected)
                _charL.interactable = false;
            if (you[1] == null || you[1].action_selected)
                _charC.interactable = false;
            if (you[2] == null || you[2].action_selected)
                _charR.interactable = false;
        }
        else
        {
            SaveEnemyActions();
        }
    }

    public void UICharSelect(int n)
    {
        step = 1;
        sel_char = n;
        _back.gameObject.SetActive(true);
        _action.gameObject.SetActive(true);
        _dir.gameObject.SetActive(false);
        _char.gameObject.SetActive(false);
        _shield.interactable = true;
        _dodgeR.interactable = false;
        _dodgeL.interactable = false;
        if (you[sel_char].shield_cooldown > 0)
            _shield.interactable = false;
        if (sel_char == 0)
        {
            if (you[1] == null)
            {
                if (you[2] == null || !you[2].dodge_next)
                    _dodgeR.interactable = true;
            }
            else if (you[1].dodge_next)
                _dodgeR.interactable = true;
        }else if(sel_char == 1)
        {
            if (you[0] == null)
                _dodgeL.interactable = true;
            if (you[2] == null)
                _dodgeR.interactable = true;
        }
        else
        {
            if (you[1] == null)
            {
                if (you[0] == null || !you[0].dodge_next)
                    _dodgeL.interactable = true;
            }
            else if (you[1].dodge_next)
                _dodgeL.interactable = true;
        }
    }

    public void UIActionSelect(int n)
    {
        Debug.Log(n);
        sel_action = n;
        if (n == 3)
        {
            step = 2;
            _back.gameObject.SetActive(true);
            _action.gameObject.SetActive(false);
            _dir.gameObject.SetActive(true);
            _char.gameObject.SetActive(false);
            _attackL.interactable = true;
            _attackR.interactable = true;
            if (sel_char == 0)
                _attackL.interactable = false;
            if (sel_char == 2)
                _attackR.interactable = false;
        }
        else
        {
            SaveActions(you, enemies);
            step = 0;
            UIShow();
        }
    }

    public void UIDirectionSelect(int n)
    {
        sel_dir = n;
        SaveActions(you, enemies);
        step = 0;
        UIShow();
    }

    void SaveActions(PP[] line, PP[] def)
    {
        line[sel_char].action_selected = true;
        switch (sel_action)
        {
            case 0: //DODGE RIGHT
                line[sel_char].DodgeNext(1);
                break;
            case 1: //DODGE LEFT
                line[sel_char].DodgeNext(-1);
                break;
            case 2: //SHIELD
                line[sel_char].ShieldUp();
                break;
            case 3: //ATTACK
                line[sel_char].AttackNext(sel_dir);
                break;
        }
    }

    void SaveEnemyActions()
    {
        // scelgo casualmente delle azioni da eseguire
        //... TODO ...
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                int x = Random.Range(0, 2);
                switch (x)
                {
                    case 0:
                        //attack
                        enemies[i].AttackNext(0);
                        break;
                    case 1:
                        //shield
                        if (enemies[i].shield_cooldown == 0)
                            enemies[i].ShieldUp();
                        else
                            enemies[i].AttackNext(0);
                        break;
                }
            }
        }

        PerformActions();
    }

    void PerformActions()
    {
        //YOU DODGE
        for (int i = 0; i < you.Length; i++)
        {
            if (you[i] != null && you[i].dodge_next)
            {
                you[i].powerful_attack = true;
                if (you[i + you[i].dodge_pos] != null)
                {
                    //solo 1 caso possibile
                    //[x,x,_]
                    you[2] = you[1];
                    you[1] = you[0];
                    you[0] = null;
                }
                else
                {
                    you[i + you[i].dodge_pos] = you[i];
                    you[i] = null;
                }
                i += 1;
            }
        }
        //ENEMY DODGE
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && enemies[i].dodge_next)
            {
                enemies[i].powerful_attack = true;
                if (enemies[i + enemies[i].dodge_pos] != null)
                {
                    //solo 1 caso possibile
                    //[x,x,_]
                    enemies[2] = enemies[1];
                    enemies[1] = enemies[0];
                    enemies[0] = null;
                }
                else
                {
                    enemies[i + enemies[i].dodge_pos] = enemies[i];
                    enemies[i] = null;
                }
            }
        }
        //YOU ATTACK
        for (int i = 0; i < you.Length; i++)
        {
            if (you[i] != null && you[i].attack_next)
            {
                Debug.Log(i + " -> " + you[i].attack_pos);
                enemies[i + you[i].attack_pos] = Attack(you[i], enemies[i + you[i].attack_pos]);
            }
        }
        //ENEMY ATTACK
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && enemies[i].attack_next)
                you[i + enemies[i].attack_pos] = Attack(enemies[i], you[i + enemies[i].attack_pos]);
        }
        //Mostro le mosse effettute prima di cancellarle dai PP
        ShowMove();
        //YOU TURN END
        for (int i = 0; i < you.Length; i++)
        {
            if (you[i] != null)
                you[i].TurnFinished();
        }
        //ENEMY TUN END
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
                enemies[i].TurnFinished();
        }
        PrintPP(you);
        PrintPP(enemies);
        if (CharAlive(you) <= 0 || CharAlive(enemies) <= 0)
        {
            UpdateShownHp();
            GameOver();
        }
        else
            UIShow();
    }

    public void UIBack()
    {
        step--;
        switch (step)
        {
            case 0:
                UIShow();
                break;
            case 1:
                UICharSelect(sel_char);
                break;
            case 2:
                UIActionSelect(sel_action);
                break;
        }
    }

    PP Attack(PP attacker, PP defender)
    {
        bool powerfulAttack = attacker.powerful_attack;
        attacker.powerful_attack = false;
        //controllo se c'è un nemico
        if (defender != null)
        {
            if (!defender.shield_next)
            {
                if (powerfulAttack)
                    defender.hp -= (2 * attacker.atk);
                else defender.hp -= attacker.atk;
                if (defender.hp <= 0)
                {
                    //se questo muore lo tolgo -> null
                    return null;
                }
            }
            else
            {
                defender.shield_cooldown = 2;
            }
            return defender;
        }
        else return null;
    }

    void GameOver()
    {
        if (CharAlive(you) <= 0)
            Debug.Log("you lost");
        else
            Debug.Log("you win");
    }
}
