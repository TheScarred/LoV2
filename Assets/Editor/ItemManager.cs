using UnityEngine;
using UnityEditor;
using System.Linq;
using Items;
using System;
using System.Collections.Generic;

public class ItemManager : EditorWindow
{
    Rect header;
    Rect weapon;
    Rect consumable;
    Rect gold;

    Texture2D headerTexture;
    Texture2D weaponTexture;
    Texture2D consumableTexture;
    Texture2D goldTexture;

    static WeaponStats weaponData;
    public static WeaponStats WeaponInfo { get { return weaponData; } }

    // Crea la ventana
    [MenuItem("Window/Item")]
    static void OpenWindow()
    {
        ItemManager window = (ItemManager)GetWindow(typeof(ItemManager));
        window.minSize = new Vector2(600, 300);
        window.Show();
    }

    private void OnEnable()
    {
        InitTextures();
        InitData();
    }
    // Toma la informacion de las clases en Item Manager
    public static void InitData()
    {
       
    }
    
    private void OnGUI()
    {
        /*DrawLayouts();
        DrawHeader();
        DrawWeaponSettings();
        DrawConsumeableSettings();
        DrawGoldSettings();*/
    }
    // Inicializa las texturas(color) de los espacios de la ventana Item MAnager
    void InitTextures()
    {
        headerTexture = new Texture2D(1, 1);
        headerTexture.SetPixel(0, 0, Color.blue);
        headerTexture.Apply();

        weaponTexture = new Texture2D(1, 1);
        weaponTexture.SetPixel(0, 0, Color.red);
        weaponTexture.Apply();

        consumableTexture = new Texture2D(1, 1);
        consumableTexture.SetPixel(0, 0, Color.green);
        consumableTexture.Apply();

        goldTexture = new Texture2D(1, 1);
        goldTexture.SetPixel(0, 0, Color.yellow);
        goldTexture.Apply();
    }

    // Define los espacios de la ventana de Item Manager

    void DrawLayouts()
    {
        header.x = 0;
        header.y = 0;
        header.width = Screen.width;
        header.height = Screen.height / 6;

        weapon.x = 0;
        weapon.y = Screen.height / 6;
        weapon.width = Screen.width / 3;
        weapon.height = Screen.height - (Screen.height / 6);

        consumable.x = Screen.width - ((Screen.width / 3) * 2);
        consumable.y = Screen.height / 6;
        consumable.width = Screen.width / 3;
        consumable.height = Screen.height - (Screen.height / 6);

        gold.x = Screen.width - (Screen.width / 3);
        gold.y = Screen.height / 6;
        gold.width = Screen.width / 3;
        gold.height = Screen.height - (Screen.height / 6);

        GUI.DrawTexture(header, headerTexture);
        GUI.DrawTexture(weapon, weaponTexture);
        GUI.DrawTexture(consumable, consumableTexture);
        GUI.DrawTexture(gold, goldTexture);
    }
    // Boton para spawnear item random
    void DrawHeader()
    {
        GUILayout.BeginArea(header);

        GUILayout.Label("Item Manager");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Random Weapon", GUILayout.Height(20)))
        {
            
        }
        if (GUILayout.Button("Spawn Random Consumable", GUILayout.Height(20)))
        {

        }
        if (GUILayout.Button("Spawn Random Gold", GUILayout.Height(20)))
        {

        }
        EditorGUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    // Enums para definir el tipo de arma e info pertinente
    void DrawWeaponSettings()
    {
        /*GUILayout.BeginArea(weapon);

        GUILayout.Label("Weapon");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Weapon Type");
        weaponData.weaponType = (WeaponType)EditorGUILayout.EnumPopup(weaponData.weaponType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Weapon Type");
        weaponData.weaponRarity = (WeaponRarity)EditorGUILayout.EnumPopup(weaponData.weaponRarity);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Spawn", GUILayout.Height(20)))
        {
            Settings.OpenWindow(Settings.SettingsType.WEAPON);
        }

        GUILayout.EndArea();
    }

    // Enums para definir el tipo de consumible e info pertinente
    void DrawConsumeableSettings()
    {
        GUILayout.BeginArea(consumable);

        GUILayout.Label("Consumable");

        GUILayout.EndArea();
    }

    // Enums para definir el tipo de oro e info pertinente
    void DrawGoldSettings()
    {
        GUILayout.BeginArea(gold);

        GUILayout.Label("Gold");

        GUILayout.EndArea();
    }
}

public class Settings : EditorWindow
{
    public enum SettingsType
    {
        WEAPON,
        CONSUMABLE,
        GOLD
    };

    static SettingsType dataSetting;
    static Settings window;
    public static void OpenWindow(SettingsType setting)
    {
        dataSetting = setting;
        window = (Settings)GetWindow(typeof(Settings));
        window.minSize = new Vector2(250, 200);
        window.Show();
    }
    private void OnGUI()
    {
        switch (dataSetting)
        {
            case SettingsType.WEAPON:
                {
                    DefineWeapon(ItemManager.WeaponInfo);
                    break;
                }
            case SettingsType.CONSUMABLE:
                {
                    DefineWeapon(ItemManager.WeaponInfo);
                    break;
                }
            case SettingsType.GOLD:
                {
                    DefineWeapon(ItemManager.WeaponInfo);
                    break;
                }
        }
    }
    void DefineWeapon(WeaponStats weaponData)
    {
        DefineModifiers(weaponData);

        /*switch (weaponData.weaponType)
        {
            case WeaponType.MELEE:
                {
                    DefineMeleeWeapon(weaponData);
                    break;
                }
            case WeaponType.RANGED:
                {
                    DefineRangedWeapon(weaponData);
                    break;
                }
        }*/
    }
    void DefineModifiers(WeaponStats weaponData)
    {
        /*if (weaponData.weaponRarity == WeaponRarity.RARE)
            ToggleModifiers(weaponData, true, false);

        else if (weaponData.weaponRarity == WeaponRarity.EPIC || weaponData.weaponRarity == WeaponRarity.LEGENDARY)
            ToggleModifiers(weaponData, true, true);

        else
            ToggleModifiers(weaponData, false, false);*/
    }
    void ToggleModifiers(WeaponStats weaponData, bool slot1, bool slot2)
    {
        //weaponData.slot1 = slot1;
        //weaponData.slot2 = slot2;
    }
    void DrawMeleeModifiers(WeaponStats weaponData)
    {
        /*if (weaponData.slot1)
        {
            if (weaponData.weaponRarity == WeaponRarity.LEGENDARY)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Legendary Melee Mod");
                weaponData.legendaryMeleeMod = (LegendaryMeleeModifier)EditorGUILayout.EnumPopup(weaponData.legendaryMeleeMod);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Melee Mod");
                weaponData.meleeMod = (MeleeModifier)EditorGUILayout.EnumPopup(weaponData.meleeMod);
                EditorGUILayout.EndHorizontal();
            }
        }
        if (weaponData.slot2)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Melee Mod");
            weaponData.meleeMod2 = (MeleeModifier)EditorGUILayout.EnumPopup(weaponData.meleeMod2);
            EditorGUILayout.EndHorizontal();
        }*/
    }
    void DrawRangedModifiers(WeaponStats weaponData)
    {
        /*if (weaponData.slot1)
        {
            if (weaponData.weaponRarity == WeaponRarity.LEGENDARY)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Legendary Melee Mod");
                weaponData.legendaryRangedMod = (LegendaryRangedModifier)EditorGUILayout.EnumPopup(weaponData.legendaryRangedMod);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Ranged Mod");
                weaponData.rangedMod = (RangedModifier)EditorGUILayout.EnumPopup(weaponData.rangedMod);
                EditorGUILayout.EndHorizontal();
            }
        }
        if (weaponData.slot2)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ranged Mod");
            weaponData.rangedMod2 = (RangedModifier)EditorGUILayout.EnumPopup(weaponData.rangedMod2);
            EditorGUILayout.EndHorizontal();
        }*/
    }
    void DefineMeleeWeapon(WeaponStats weaponData)
    {
        /*switch (weaponData.weaponRarity)
        {
            case WeaponRarity.UNCOMMON:
                {
                    DrawSlider("Damage", weaponData, 5.5f, 6.5f);
                    break;
                }
            case WeaponRarity.RARE:
                {
                    DrawSlider("Damage", weaponData, 6f, 7f);
                    DrawMeleeModifiers(weaponData);
                    break;
                }
            case WeaponRarity.EPIC:
                {
                    DrawSlider("Damage", weaponData, 6.5f, 7.5f);
                    DrawMeleeModifiers(weaponData);
                    break;
                }
            case WeaponRarity.LEGENDARY:
                {
                    DrawSlider("Damage", weaponData, 7f, 8f);
                    DrawMeleeModifiers(weaponData);
                    break;
                }
        }*/
    }
    void DefineRangedWeapon(WeaponStats weaponData)
    {
        /*switch (weaponData.weaponRarity)
        {
            case WeaponRarity.UNCOMMON:
                {
                    DrawSlider("Damage", weaponData, 3f, 3.5f);
                    break;
                }
            case WeaponRarity.RARE:
                {
                    DrawSlider("Damage", weaponData, 3.5f, 4f);
                    DrawRangedModifiers(weaponData);
                    break;
                }
            case WeaponRarity.EPIC:
                {
                    DrawSlider("Damage", weaponData, 4f, 4.5f);
                    DrawRangedModifiers(weaponData);
                    break;
                }
            case WeaponRarity.LEGENDARY:
                {
                    DrawSlider("Damage", weaponData, 4.5f, 5f);
                    DrawRangedModifiers(weaponData);
                    break;
                }
        }*/
    }
    void DrawSlider(string name ,WeaponStats weaponData, float min, float max)
    {
        EditorGUILayout.BeginHorizontal();
        //weaponData.damage = EditorGUILayout.Slider(name, weaponData.damage, min, max);
        EditorGUILayout.EndHorizontal();
    }
}