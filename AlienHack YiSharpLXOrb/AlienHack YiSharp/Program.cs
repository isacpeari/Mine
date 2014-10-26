﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;


namespace AlienHack_YiSharp
{
    internal class Program
    {

        public static Spell Q, W, E, R;
        public static List<Spell> SpellList = new List<Spell>();
        public static Obj_AI_Hero Player = ObjectManager.Player, TargetObj = null;
        public static SpellSlot IgniteSlot;
        public static Items.Item Tiamat = new Items.Item(3077, 375);
        public static Items.Item Hydra = new Items.Item(3074, 375);
        public static Items.Item BladeOfRuinKing = new Items.Item(3153, 450);
        public static Items.Item BlidgeWater = new Items.Item(3144, 450);
        public static Items.Item Youmuu = new Items.Item(3142, 200);
        public static Menu Config;
        public static String Name;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Name = Player.ChampionName;
            if (Name != "MasterYi") return;

            SpellDataInst qData = Player.Spellbook.GetSpell(SpellSlot.Q);
            SpellDataInst wData = Player.Spellbook.GetSpell(SpellSlot.W);
            SpellDataInst eData = Player.Spellbook.GetSpell(SpellSlot.E);
            SpellDataInst rData = Player.Spellbook.GetSpell(SpellSlot.R);

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 175);
            E = new Spell(SpellSlot.E, 175);
            R = new Spell(SpellSlot.R, 175);

            Config = new Menu("AlienHack_" + Name, "AlienHack_" + Name, true);

            //Orbwalker submenu
            var orbwalkerMenu = new Menu("My Orbwalker", "my_Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);
            
         
            //Add the target selector to the menu as submenu.
            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Load the orbwalker and add it to the menu as submenu.

            //LaneClear
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQLaneClear", "Use Q").SetValue(true));
            Config.SubMenu("LaneClear")
                .AddItem(
                    new MenuItem("LaneClear_Key", "LaneClear!").SetValue(
                        new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Harass menu:
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E").SetValue(false));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("Harass_Key", "Harass!").SetValue(
                        new KeyBind(Config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Combo menu:
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            //Config.AddItem(new MenuItem("MinQRange", "Min Q range").SetValue(new Slider(600, 0, 600)));
            Config.SubMenu("Combo")
                .AddItem(new MenuItem("MinQRange", "Min Q Range").SetValue(new Slider(600, 0, 600)));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            Config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("Combo_Key", "Combo!").SetValue(
                        new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoTiamat", "Auto Tiamat").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoBOTRK", "Auto BOTRK").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoYoumuu", "Auto Youmuu").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoIgnite", "Auto Ignite").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoQSteal", "Auto Q Steal").SetValue(true));

            Config.AddToMainMenu();
            // End Menu

            IgniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");

            Game.PrintChat("AlienHack [YiSharp - WujuMaster] Loaded!");
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static int getQRange()
        {
            return Config.Item("MinQRange").GetValue<Slider>().Value;
        }

        private static bool IsQSteal()
        {
            if (Config.Item("AutoQSteal").GetValue<bool>())
            {
                return Q.IsReady();
            }
            return false;
        }

        private static bool IsTiamat()
        {
            if (Config.Item("AutoTiamat").GetValue<bool>())
            {
                return Tiamat.IsReady();
            }
            return false;
        }

        private static bool IsIgnite()
        {
            if (Config.Item("AutoIgnite").GetValue<bool>())
            {
                if (Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsHydra()
        {
            if (Config.Item("AutoTiamat").GetValue<bool>())
            {
                return Hydra.IsReady();
            }
            return false;
        }

        private static bool IsBotrk()
        {
            if (Config.Item("AutoBOTRK").GetValue<bool>())
            {
                return BladeOfRuinKing.IsReady();
            }
            return false;
        }

        private static bool IsBilge()
        {
            if (Config.Item("AutoBOTRK").GetValue<bool>())
            {
                return BlidgeWater.IsReady();
            }
            return false;
        }

        private static bool IsYoumuu()
        {
            if (Config.Item("AutoYoumuu").GetValue<bool>())
            {
                return Youmuu.IsReady();
            }
            return false;
        }

        private static bool IsQLaneClear()
        {
            if (Config.Item("UseQLaneClear").GetValue<bool>())
            {
                return Q.IsReady();
            }
            return false;
        }

        private static bool IsQHarass()
        {
            if (Config.Item("UseQHarass").GetValue<bool>())
            {
                return Q.IsReady();
            }
            return false;
        }

        private static bool IsEHarass()
        {
            if (Config.Item("UseEHarass").GetValue<bool>())
            {
                return E.IsReady();
            }
            return false;
        }

        private static bool IsQCombo()
        {
            if (Config.Item("UseQCombo").GetValue<bool>())
            {
                return Q.IsReady();
            }
            return false;
        }

        private static bool IsECombo()
        {
            if (Config.Item("UseECombo").GetValue<bool>())
            {
                return E.IsReady();
            }
            return false;
        }

        private static bool IsRCombo()
        {
            if (Config.Item("UseRCombo").GetValue<bool>())
            {
                return R.IsReady();
            }
            return false;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            switch (LXOrbwalker.CurrentMode)
            {
                case LXOrbwalker.Mode.LaneClear:
                    DoLaneClear();
                    break;
                case LXOrbwalker.Mode.Harass:
                    DoHarass();
                    break;
                case LXOrbwalker.Mode.Combo:
                    DoCombo();
                    break;
            }

            Obj_AI_Hero target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            //Find All Minion
            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> jungleMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.Neutral);
            allMinions.AddRange(jungleMinions);

            //Auto Ignite
            if (IsIgnite() && Player.Distance(target) <= 600)
            {
                if (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health)
                {
                    Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                }
            }

            //AutoQ
            if (IsQSteal() && Player.Distance(target) <= Q.Range)
            {
                if (Player.GetSpellDamage(target, SpellSlot.Q) > target.Health)
                {
                    Q.Cast(target);
                }
            }

            //AUTO DODGE
        }

        private static void DoCombo()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            if (IsQCombo() && Player.Distance(target) >= getQRange())
            {
                Q.Cast(target);
            }

            if (IsECombo() && E.Range >= Player.Distance(target))
            {
                E.Cast();
            }

            if (IsRCombo() && R.Range >= Player.Distance(target))
            {
                R.Cast();
            }

            if (IsTiamat() && Tiamat.Range >= Player.Distance(target))
            {
                Tiamat.Cast();
            }

            if (IsHydra() && Hydra.Range >= Player.Distance(target))
            {
                Hydra.Cast();
            }

            if (IsBotrk() && BladeOfRuinKing.Range >= Player.Distance(target))
            {
                if (Player.Health <= Player.MaxHealth - target.MaxHealth*0.1)
                {
                    BladeOfRuinKing.Cast(target);
                }
            }

            if (IsBilge() && BlidgeWater.Range >= Player.Distance(target))
            {
                BlidgeWater.Cast(target);
            }

            if (IsYoumuu() && Youmuu.Range >= Player.Distance(target))
            {
                Youmuu.Cast();
            }
        }

        private static void DoHarass()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            if (IsQHarass() && Q.Range >= Player.Distance(target))
            {
                Q.Cast(target);
            }

            if (IsEHarass() && E.Range >= Player.Distance(target))
            {
                E.Cast();
            }

            if (IsTiamat() && Tiamat.Range >= Player.Distance(target))
            {
                Tiamat.Cast();
            }

            if (IsHydra() && Hydra.Range >= Player.Distance(target))
            {
                Hydra.Cast();
            }

            //SHOULD BE ON COMBO MODE
            /*if (IsBOTRK() && BladeOfRuinKing.Range > Player.Distance(target))
            {
                if (Player.Health < Player.MaxHealth - target.MaxHealth * 0.1)
                {
                    BladeOfRuinKing.Cast(target);
                }
            }

            if (IsYoumuu() && Youmuu.Range > Player.Distance(target))
            {
                Youmuu.Cast();
            }*/
        }

        private static void DoLaneClear()
        {
            //Find All Minion
            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> jungleMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.Neutral);
            allMinions.AddRange(jungleMinions);

            //Auto Q
            if (IsQLaneClear() && allMinions.Count > 0)
            {
                foreach (
                    Obj_AI_Base minion in
                        allMinions.Where(minion => minion.IsValidTarget())
                            .Where(minion => Q.Range >= Player.Distance(minion))
                            .OrderBy(minion => Player.Distance(minion)))
                {
                    Q.Cast(minion);
                    break;
                }
            }

            //Auto Tiamat
            if (IsTiamat() && allMinions.Count > 0)
            {
                foreach (
                    Obj_AI_Base minion in
                        allMinions.Where(minion => minion.IsValidTarget())
                            .Where(minion => Tiamat.Range >= Player.Distance(minion)))
                {
                    Tiamat.Cast();
                    break;
                }
            }

            //Auto Hydra
            if (IsHydra() && allMinions.Count > 0)
            {
                foreach (
                    Obj_AI_Base minion in
                        allMinions.Where(minion => minion.IsValidTarget())
                            .Where(minion => Hydra.Range >= Player.Distance(minion)))
                {
                    Hydra.Cast();
                    break;
                }
            }
        }
    }
}