using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LX_Orbwalker;

namespace AlienHack_YiSharp
{
    internal class Program
    {
        public static Orbwalking.Orbwalker Orbwalker;
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
            
            IgniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");

            Config = new Menu("AlienHack [" + Name + "]", "AlienHack_" + Name, true);

            //Lxorbwalker
            /*var orbwalkerMenu = new Menu("Orbwalker", "LX_Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);*/

            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            //Add the target selector to the menu as submenu.
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Misc
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoIgnite", "Auto Ignite").SetValue(true));

            Config.AddToMainMenu();
            // End Menu

            Game.PrintChat("AlienHack [OrbwalkerLX] Loaded!");
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static void Ks()
        {
            List<Obj_AI_Hero> nearChamps = (from champ in ObjectManager.Get<Obj_AI_Hero>()
                where Player.Distance(champ.ServerPosition) <= 600 && champ.IsEnemy
                select champ).ToList();
            nearChamps.OrderBy(x => x.Health);

            foreach (Obj_AI_Hero target in nearChamps)
            {
                //ignite
                if (target != null && IsIgnite() && Player.Distance(target.ServerPosition) <= 600)
                {
                    if (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health)
                    {
                        Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                    }
                }
            }
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
                    //Game.PrintChat("Ignite Enabled");
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

        private static bool IsBOTRK()
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

        private static bool IsWCombo()
        {
            if (Config.Item("UseWCombo").GetValue<bool>())
            {
                return W.IsReady();
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
            Ks();
        }
    }
}