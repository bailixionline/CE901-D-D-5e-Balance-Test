using System;

using System.Collections;
using System.Collections.Generic;

namespace DnDTestProgram
{
    class MainClass
    {
        static int sleepTime = 0;        

        static int Str, Dex, Con, Int, Wis, Cha;
        static int Str_Mod, Dex_Mod, Con_Mod, Int_Mod, Wis_Mod, Cha_Mod;
        static int proficiencyBouns;
        static List<string> proficiencyWeaponList = new List<string> { };
        
        //           体育,     骗人的戏法，   隐身，   奥秘，  历史，   自然，  宗教，       调查，        动物处理，    洞察，   医学，     知觉，      生存，    欺骗，     恐吓，          表演，     说服 
        static int Athletics, SleightofHand, Stealth, Arcana, History, Nature, Religion, Investigation, AnimalHandling, Insight, Medicine, Perception, Survival, Deception, Intimidation, Performance, Persuasion;

        static List<int> SkillsList = new List<int> { };
        //                                                  体育 骗人的戏法 隐身 奥秘 历史 自然 宗教 调查 动物处理 洞察 医学 知觉 生存 欺骗 恐吓 表演 说服 
        static double[] ExplorationSkillsInfluence =       { 1,      0,     0.8, 0.8, 0.2,  1,  0.2, 0.8,    0.9,  0.1, 0.6, 0.2,  1,   0,    0,  0,   0, };
        static double[] SocialInteractionSkillsInfluence = { 0,      1,     0.2, 0.2, 0.8,  0,  0.8, 0.2,    0.1,  0.9, 0.6, 0.8,  0,   1,    1,  1,   1, };

        static int attackSuccessTimes = 0;
        static bool isCritSuc = false; //is critical success
        //static bool isCritFub = false; //is critical fumble not official         

        static List<Weapon> weapons = new List<Weapon>();
        static string[] meleeWeaponList = { "Club", "Dagger", "Greatclub", "Handaxe", "Javelin", "Light hammer", "Light hammer", "Mace", "Quarterstaff", "Sickle", "Spear" };
        static string[] rangeWeaponList = { "Crossbow,light", "Dart", "Shortbow", "Sling" };
        static string[] martialMeleeWeaponList = { "Battleaxe", "Flail", "Glavie", "GreatAxe", "Greatsword", "Halberd", "Lance", "Longsword", "Maul", "Morningstar", "Pike", "Rapier", "Scimitar", "Shortword", "Trident", "War pick", "Warhammer", "Whip" };
        static string[] martialRangedWeaponList = { "Blowgun", "Crossbow,hand", "Crossbow,heavy", "Longbow", "Net" };

        static List<Enemy> enemies = new List<Enemy>();

        static List<Player> players = new List<Player>();

        public enum NAD
        {
            Normal,
            Advantage,
            Disadvantage
        }

        public static void v1Main(string[] args)
        {
            ////--------------Roll dice Check -----------------------//
            //for (int i = 0; i < 10; i++)                           //
            //{                                                      //
            //    int result = RollDiceMethod2(10);                  //
            //    Console.WriteLine("D" + i + ": result_" + result); //
            //    System.Threading.Thread.Sleep(sleepTime);          //
            //}                                                      //
            //Console.ReadLine();                                    //
            /////////////////////////////////////////////////////////// 


            ////----------initial wapon and enemies----------------------
            AddWeapons();
            AddEnemies();

            ////-----------how to set a player character's ability---------
            ////set ability
            ////approach 1) 
            ////    e.g. GetAbility() 
            ////approach 2) 
            ////    e.g. SetAbility(15, 14, 13, 12, 10,8);

            ////--------how to set a player character's extral ability-----
            ////-----------------(bouns form class and race)---------------
            ////Change number below
            ////    e.g. ExtraAbility(0, 0, 0, 0, 0, 0);

            ////------how to calculate a player character's ablity modify--
            //CalculateMod();

            ////-------how to set player character's proficiency-----------
            ////    e.g. proficiencyBouns = 2;

            ////-------how to set player character proficiency weapon------
            ////1) Clear list
            //proficiencyWeaponList.Clear();
            ////2) Add name to the list where you can chose from 
            ////   a)meleeWeaponList
            ////   b)rangeWeaponList
            ////   c)martialMeleeWeaponList
            ////   d)martialRangedWeaponList
            ////   e)add the wapon name to(new List<string>() {"weapon name"})
            ////        
            ////    e.g. AddProficiencyWeaponList(meleeWeaponList);
            ////    e.g. AddProficiencyWeaponList(new List<string>() {"weapon name"});

            ////-------how to set a player character's skills modify-------
            ////    e.g. CalculateSkills();

            ////--------how to set a player character's extral skills------
            ////-----------------(bouns form class and race)---------------
            ////Wtite down name, and bounds
            ////skills name can chose from 
            ////   a)Athletics
            ////   b)SleightofHand
            ////   c)Stealth
            ////   d)Arcana
            ////   e)History
            ////   f)Nature
            ////   g)Religion
            ////   h)Investigation
            ////   i)AnimalHandling
            ////   j)Insight
            ////   k)Medicine
            ////   l)Perception
            ////   m)Survival
            ////   n)Deception
            ////   o)Intimidation
            ////   p)Performance
            ////   q)Persuasion;
            ////       e.g. ExtraSkills("Athletics", 1);

            //-----------------------------------------------------------------------------------------------------------------------------------
            /*
            ////----------how Success to pass a quest in different modify level------
            ////folowing code will calculate from ability modify -5 to +5 under the normal statue
            SetAbility(0, 2, 4, 6, 8, 10); // modify -5, -4, -3, -2, -1, 0
            CalculateMod();
            Console.WriteLine("Normal statue");
            AbilityCheck(1000, NAD.Normal, NAD.Normal, NAD.Normal, NAD.Normal, NAD.Normal, NAD.Normal);
            SetAbility(12, 14, 16, 18, 20, 20); // modify 1, 2, 3, 4, 5, 5
            CalculateMod();
            AbilityCheck(1000, NAD.Normal, NAD.Normal, NAD.Normal, NAD.Normal, NAD.Normal, NAD.Normal);
            ////folowing code will calculate from ability modify -5 to +5 under the advantage statue
            SetAbility(0, 2, 4, 6, 8, 10); // modify -5, -4, -3, -2, -1, 0
            CalculateMod();
            Console.WriteLine("Advantage statue");
            AbilityCheck(1000, NAD.Advantage, NAD.Advantage, NAD.Advantage, NAD.Advantage, NAD.Advantage, NAD.Advantage);
            SetAbility(12, 14, 16, 18, 20, 20); // modify 1, 2, 3, 4, 5, 5
            CalculateMod();
            Console.WriteLine("Advantage statue");
            AbilityCheck(1000, NAD.Advantage, NAD.Advantage, NAD.Advantage, NAD.Advantage, NAD.Advantage, NAD.Advantage);
            ////folowing code will calculate from ability modify -5 to +5 under the disadvantage statue
            SetAbility(0, 2, 4, 6, 8, 10); // modify -5, -4, -3, -2, -1, 0
            CalculateMod();
            Console.WriteLine("Disadvantage statue");
            AbilityCheck(1000, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage);
            SetAbility(12, 14, 16, 18, 20, 20); // modify 1, 2, 3, 4, 5, 5
            CalculateMod();
            Console.WriteLine("Disadvantage statue");
            AbilityCheck(1000, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage);
            Console.ReadLine(); 

            ////--------------Weapon damage rate test---------------------------------------------------------------                                                                                 
            ////chose from 1. meleeWeaponList 2.rangeWeaponList 3.martialMeleeWeaponList 4.martialRangedWeaponList  
            AllWeaponCheck(meleeWeaponList);    
            AllWeaponCheck(rangeWeaponList);   
            AllWeaponCheck(martialMeleeWeaponList);   
            AllWeaponCheck(martialRangedWeaponList);   
            Console.ReadLine();   
            */
            ////------------SavingthrowTest---------
            SavingThrownTest(10000);            
            Console.ReadLine();  
                            




            //------------------------------------prototype of version 2---------------------------------------------
            //double result = 0;
            //1. exploration
            //result += SkillsCheck(ExplorationSkillsInfluence,1000);
            //Console.WriteLine("\n1. exploration test finished, final result: \n"+ result);

            //2. social interaction
            //result += SkillsCheck(SocialInteractionSkillsInfluence, 1000);
            //Console.WriteLine("\n2. social interaction test finished, final result: \n" + result);

            //3. combat


            //AbilityCheck("Str", NAD.Normal, 100);
            //AbilityCheck("Dex", NAD.Normal, 100);
            //AbilityCheck("Con", NAD.Normal, 100);
            //AbilityCheck("Int", NAD.Normal, 100);
            //AbilityCheck("Wis", NAD.Normal, 100);
            //AbilityCheck("Cha", NAD.Normal, 100);



            //AbilityCheck(10000,NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage);       

            //Console.WriteLine("-------------------------------");

            //SetAbility(12, 14, 16, 18, 20, 22);
            //CalculateMod();
            //PrintAblityAndModify();
            //AbilityCheck(10000, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage, NAD.Disadvantage);
            //Console.WriteLine("-------------------------------");


            //for combat test
            //ACandDamageCheck(1000, 0, 5, 1, 8, 14);
            //ACandDamageCheck(10000, 0, "GreatAxe", 14);
            //Console.WriteLine(GetWeaponDamageRate(10000, 0, "GreatAxe", 14));
            //ACandDamageCheck(1000, 0, "Glaive", 14);




            //Console.ReadLine();

        }
        
        /*
        public static void ACandDamageCheck(int times, int Normal0_Advantage1_Disadvantage2, int modBouns, int weaponBouns, int diceNumber, int diceType, int ArmorClass)
        {
            int ACSuccessTimes = AttacCheckSuccessTimes(times, Normal0_Advantage1_Disadvantage2, modBouns, ArmorClass);
            int totalDamage = DamageCheck(ACSuccessTimes, Normal0_Advantage1_Disadvantage2, diceNumber, diceType, modBouns,weaponBouns, ArmorClass);
            double damageRate = Convert.ToDouble(totalDamage) / Convert.ToDouble(times);
            Console.WriteLine("AC and Damage Check: \n" +
                              "  - Test times:                      " + times + "\n" +
                              "  - AC Success Times:                " + ACSuccessTimes + "\n" +
                              "  - TotalDamage:                     " + totalDamage + "\n" +
                              "  - Damage Rate(Total Damage/Times): " + damageRate);
        }

        //times return total damage
        public static int DamageCheck(int times, NAD statue, int diceNumber, int diceType, int modBouns, int weaponBouns, int ArmorClass)
        {
            int totalDamage = 0;
            for (int i = 0; i < times; i++)
            {
                totalDamage += DamageCheck(statue, diceNumber, diceType, modBouns, weaponBouns);
                System.Threading.Thread.Sleep(sleepTime);
            }
            return totalDamage;
        }

        //return single damage
        public static int DamageCheck(NAD statue, int diceNumber, int diceType, int modBouns, int weaponBouns)
        {
            int damage = 0;  
            for (int i = 0; i < diceNumber; i++)
            {
                damage += RollDiceMethod2(diceType);
                System.Threading.Thread.Sleep(sleepTime);
            }
            damage += modBouns;
            damage += weaponBouns;
            int critBouns = (isCrit) ? (RollDiceMethod2(diceType)) : 0;
            return damage + critBouns;
        }
        */

        public static void SavingThrownTest(int times)
        {
            int TotalSuccessTimes = 0;
            for (int i = 0; i < times; i++)
            {
                int success = 0;
                int fail = 0;

                while (success < 3 && fail < 3)
                {
                    int result = RollDiceMethod2(20);
                    success = (result == 20) ? success + 3 : success;
                    fail = (result == 1) ? fail + 2 : fail;

                    success = (result >= 10 && result != 20) ? success + 1 : success;
                    fail = (result < 10 && result != 1) ? fail + 1 : fail;
                    //Console.WriteLine(success + " " + fail);
                }
                //Console.WriteLine(success > fail);
                TotalSuccessTimes = (success > fail) ? TotalSuccessTimes + 1 : TotalSuccessTimes;
            }
            Console.WriteLine("Total test Times:    " + times);
            Console.WriteLine("Total Success Times: " + TotalSuccessTimes);
            Console.WriteLine("Total fail Times:    " + (times - TotalSuccessTimes));
        }
        public static void AddProficiencyWeaponList(string[] list)
        {
            int length = list.Length;
            for (int i = 0; i < length; i++)
            {
                proficiencyWeaponList.Add(list[i]);
            }
        }

        //-------------------------------Calculate for initial------------------------------------------------------
        //-----------------------------------Print result-----------------------------------------------------------

        public static void GetAbility()
        {
            Console.WriteLine("Please enter your Strength");
            Str = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter your Dexterity");
            Dex = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter your Constitution");
            Con = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter your Intelligence");
            Int = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter your Wisdom");
            Wis = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter your Charisma");
            Cha = Convert.ToInt32(Console.ReadLine());
        }
        public static void SetAbility(int StrNum, int DexNum, int ConNum, int IntNum, int WisNum, int ChaNum)
        {
            Str = StrNum;
            Dex = DexNum;
            Con = ConNum;
            Int = IntNum;
            Wis = WisNum;
            Cha = ChaNum;
        }
        public static void ExtraAbility(int StrNum, int DexNum, int ConNum, int IntNum, int WisNum, int ChaNum)
        {
            Str += StrNum;
            Dex += DexNum;
            Con += ConNum;
            Int += IntNum;
            Wis += WisNum;
            Cha += ChaNum;
        }
        public static void ExtraSkills(String skill, int proficiency)
        { 
            switch (skill)
            {
                case "Athletics":
                    Athletics += proficiency;
                    break;
                case "SleightofHand":
                    SleightofHand += proficiency;
                    break;
                case "Stealth":
                    Stealth += proficiency;
                    break;
                case "Arcana":
                    Arcana += proficiency;
                    break;
                case "History":
                    History += proficiency;
                    break;
                case "Nature":
                    Nature += proficiency;
                    break;
                case "Religon":
                    Religion += proficiency;
                    break;
                case "Investigation":
                    Investigation += proficiency;
                    break;
                case "AnimalHandling":
                    AnimalHandling += proficiency;
                    break;
                case "Insight":
                    Insight += proficiency;
                    break;
                case "Medicine":
                    Medicine += proficiency;
                    break;
                case "Perception":
                    Perception += proficiency;
                    break;
                case "Survival":
                    Survival += proficiency;
                    break;
                case "Deception":
                    Deception += proficiency;
                    break;
                case "Intimidation":
                    Intimidation += proficiency;
                    break;
                case "Performance":
                    Performance += proficiency;
                    break;
                case "Persuasion":
                    Persuasion += proficiency;
                    break;
            }
        }

        public static void PrintAblityAndModify()
        {
            Console.WriteLine("Your ability |  Modify" + "\n" +
                             "  - Str. " + Str + "   |  " + Str_Mod + "\n" +
                             "  - Dex. " + Dex + "   |  " + Dex_Mod + "\n" +
                             "  - Con. " + Con + "   |  " + Con_Mod + "\n" +
                             "  - Int. " + Int + "   |  " + Int_Mod + "\n" +
                             "  - Wis. " + Wis + "   |  " + Wis_Mod + "\n" +
                             "  - Cha. " + Cha + "   |  " + Cha_Mod + "\n"
                            );
        }
        public static void PrintAbility()
        {
            Console.WriteLine("Your ability are\n" +
                              "  - Str. " + Str + "\n" +
                              "  - Dex. " + Dex + "\n" +
                              "  - Con. " + Con + "\n" +
                              "  - Int. " + Int + "\n" +
                              "  - Wis. " + Wis + "\n" +
                              "  - Cha. " + Cha + "\n"
                             );
        }
        public static void PrintAbilityModify()
        {
            Console.WriteLine("Your ability modify are\n" +
                             "  - Str.Mod " + Str_Mod + "\n" +
                             "  - Dex.Mod " + Dex_Mod + "\n" +
                             "  - Con.Mod " + Con_Mod + "\n" +
                             "  - Int.Mod " + Int_Mod + "\n" +
                             "  - Wis.Mod " + Wis_Mod + "\n" +
                             "  - Cha.Mod " + Cha_Mod + "\n"
                            );
        }
        public static void PrintAbilityCheck(String type, NAD nadState, int times)
        {
            Console.WriteLine(type + " "+ GetValue(type) +" Ability Check Result:" +
                              "\n  - VeryEasy:         " + AbilityCheck(type, times, nadState, 5) +
                              "\n  - Easy:             " + AbilityCheck(type, times, nadState, 10) +
                              "\n  - Medium:           " + AbilityCheck(type, times, nadState, 15) +
                              "\n  - Hard:             " + AbilityCheck(type, times, nadState, 20) +
                              "\n  - VeryHard:         " + AbilityCheck(type, times, nadState, 25) +
                              "\n  - NearlyImpossible: " + AbilityCheck(type, times, nadState, 30));

            //Console.WriteLine(type + " Ability Check Result:" +
            //                  "\n" + AbilityCheck(type, times, nadState, 5) +
            //                  "\n" + AbilityCheck(type, times, nadState, 10) +
            //                  "\n" + AbilityCheck(type, times, nadState, 15) +
            //                  "\n" + AbilityCheck(type, times, nadState, 20) +
            //                  "\n" + AbilityCheck(type, times, nadState, 25) +
            //                  "\n" + AbilityCheck(type, times, nadState, 30));
        }

        public static int GetValue(string ability)
        {
            switch (ability)
            {
                case "Str":
                    return Str_Mod;
                case "Dex":
                    return Dex_Mod;
                case "Int":
                    return Int_Mod;
                case "Con":
                    return Con_Mod;
                case "Cha":
                    return Cha_Mod;
                case "Wis":
                    return Wis_Mod;
            }
            return 0;
        }
        public static void PrintSkills()
        {
            Console.WriteLine("Your skills are\n" +
                              "  - Athletics         " + Athletics +"\n" +
                              "  - SleightofHand     " + SleightofHand +"\n" +
                              "  - Stealth           " + Stealth +"\n" +
                              "  - Arcana            " + Arcana +"\n" +
                              "  - History           " + History +"\n" +
                              "  - Nature            " + Nature +"\n" +
                              "  - Religon           " + Religion +"\n" +
                              "  - Investigation     " + Investigation +"\n" +
                              "  - AnimalHandling    " + AnimalHandling +"\n" +
                              "  - Insight           " + Insight +"\n" +
                              "  - Medicine          " + Medicine +"\n" +
                              "  - Perception        " + Perception +"\n" +
                              "  - Survival          " + Survival +"\n" +
                              "  - Deception         " + Deception +"\n" +
                              "  - Intimidation      " + Intimidation +"\n" +
                              "  - Performance       " + Performance + "\n" +
                              "  - Persuasion        " + Persuasion + "\n");
        }

        public static void CalculateMod()
        {
            Str_Mod = (Str > 10) ? (int)Math.Floor((Str - 10) / 2 + 0f) : (int)Math.Floor((Str - 1 - 10) / 2 + 0f);
            Dex_Mod = (Dex > 10) ? (int)Math.Floor((Dex - 10) / 2 + 0f) : (int)Math.Floor((Dex - 1 - 10) / 2 + 0f);
            Con_Mod = (Con > 10) ? (int)Math.Floor((Con - 10) / 2 + 0f) : (int)Math.Floor((Con - 1 - 10) / 2 + 0f);
            Int_Mod = (Int > 10) ? (int)Math.Floor((Int - 10) / 2 + 0f) : (int)Math.Floor((Int - 1 - 10) / 2 + 0f);
            Wis_Mod = (Cha > 10) ? (int)Math.Floor((Wis - 10) / 2 + 0f) : (int)Math.Floor((Wis - 1 - 10) / 2 + 0f);
            Cha_Mod = (Cha > 10) ? (int)Math.Floor((Cha - 10) / 2 + 0f) : (int)Math.Floor((Cha - 1 - 10) / 2 + 0f);
        }
        public static void CalculateSkillsList()
        {
            SkillsList.Clear();
            SkillsList.Add(Athletics);
            SkillsList.Add(SleightofHand);
            SkillsList.Add(Stealth);
            SkillsList.Add(Arcana);
            SkillsList.Add(History);
            SkillsList.Add(Nature);
            SkillsList.Add(Religion);
            SkillsList.Add(Investigation);
            SkillsList.Add(AnimalHandling);
            SkillsList.Add(Insight);
            SkillsList.Add(Medicine);
            SkillsList.Add(Perception);
            SkillsList.Add(Survival);
            SkillsList.Add(Deception);
            SkillsList.Add(Intimidation);
            SkillsList.Add(Performance);
            SkillsList.Add(Persuasion);
        }
        public static void CalculateSkills()
        {
            //Str
            Athletics = Str_Mod;

            //Dex
            SleightofHand = Dex_Mod;
            Stealth = Dex_Mod;

            //Con

            //Int
            Arcana = Int_Mod;
            History = Int_Mod;
            Nature = Int_Mod;
            Religion = Int_Mod;
            Investigation = Int_Mod;

            //Wis
            AnimalHandling = Wis_Mod;
            Insight = Wis_Mod;
            Medicine = Wis_Mod;
            Perception = Wis_Mod;
            Survival = Wis_Mod;

            //Cha
            Deception = Cha_Mod;
            Intimidation = Cha_Mod;
            Performance = Cha_Mod;
            Persuasion = Cha_Mod;
        }


        //-------------------------------combat check------------------------------------------------------
        public static int TotalDamage(int times, NAD statue, int diceNumber, int diceType, int modBouns, int weaponBouns, int armorClass)
        {
            int totalDamage = 0;
            attackSuccessTimes = 0; // reset
            for (int i = 0; i < times; i++)
            {
                int damage = 0;
                if (AttacCheck(statue, modBouns, armorClass))
                {
                    for (int m = 0; m < diceNumber; m++)
                    {
                        damage += RollDiceMethod2(diceType);
                        System.Threading.Thread.Sleep(sleepTime);
                    }
                    attackSuccessTimes += 1;
                }
                damage += modBouns;
                damage += weaponBouns;
                int critBouns = (isCritSuc) ? (RollDiceMethod2(diceType)) : 0;
                damage += critBouns;
                totalDamage += damage;
            }
            return totalDamage;
        }
        //public static int AttacCheckSuccessTimes(int times, NAD statue, int bouns, int ArmorClass)
        //{
        //    int successTimes = 0;
        //    for (int i = 0; i < times; i++)
        //    {
        //        successTimes = (AttacCheck(statue, bouns, ArmorClass)) ? successTimes + 1 : successTimes;
        //        System.Threading.Thread.Sleep(sleepTime);
        //    }
        //    double result = Convert.ToDouble(successTimes) / Convert.ToDouble(times);
        //    return successTimes;
        //}
        public static void AttacCheckPercentage(int times, NAD statue, int bouns, int ArmorClass)
        {
            int successTimes = 0;
            for (int i = 0; i < times; i++)
            {
                successTimes = (AttacCheck(statue, bouns, ArmorClass)) ? successTimes + 1 : successTimes;
                System.Threading.Thread.Sleep(sleepTime);
            }
            double result = Convert.ToDouble(successTimes) / Convert.ToDouble(times);
            Console.WriteLine("AC check result: (Bouns: " + bouns + " /EnemyArmorClass: " + ArmorClass + ")\n" +
                              "  - " + result);
        }
        // check if the attack 1)sucusse or not, miss or not, 2)is cretic or not
        public static bool AttacCheck(NAD statue, int bouns, int ArmorClass)
        {
            int d20Result = 0;
            int d20Result2 = 0;
            int attack = 0;

            d20Result = RollDiceMethod2(20);
            System.Threading.Thread.Sleep(sleepTime);
            d20Result2 = RollDiceMethod2(20);
            //critical check
            isCritSuc = (d20Result == 20);
            //isCritFub = (d20Result == 1); // not official
            switch (statue)
            {
                case NAD.Normal://Normal
                    //if (isCritFub) // not official
                    //    return false;
                    attack = d20Result + bouns;
                    return (attack > (ArmorClass - 1));
                case NAD.Advantage: //Advantage
                    attack = (d20Result > d20Result2) ? d20Result + bouns : d20Result2 + bouns;
                    isCritSuc = (attack == 20);
                    return (attack > (ArmorClass - 1));
                case NAD.Disadvantage: //Disadvantage
                    attack = (d20Result < d20Result2) ? d20Result + bouns : d20Result2 + bouns;
                    isCritSuc = (attack == 20);
                    return (attack > (ArmorClass - 1));
                    //return (d20Result ==1 || d20Result2 ==1)? false : (attack > (ArmorClass - 1)); // not official
            }
            return false;
        }
        //weapons
        public static void AllWeaponCheck(String[] weaponList)
        {
            for (int t = 0; t < weaponList.Length; t++)
            {
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine("-------------------  " + weaponList[t] + "  --------------------");
                Console.WriteLine("");
                for (int j = 0; j < 21; j = j + 2)
                {
                    Console.WriteLine("--------------------------------------------------------------------");
                    SetAbility(j, j, j, j, j, j);
                    CalculateMod();
                    Console.WriteLine(Str_Mod);
                    for (int k = 0; k < 21; k++)
                    {
                        Console.WriteLine(GetWeaponDamageRate(100000, NAD.Normal, weaponList[t], k));
                    }
                }
            }
        }
        public static void ACandDamageCheck(int times, NAD statue, string weapon, int armorClass)
        {

            int modBouns = 0;
            if (FindWeapon(weapon).isMelee)
                modBouns = (FindWeapon(weapon).isFiness) ? ((Str_Mod > Dex_Mod) ? Str_Mod : Dex_Mod) : Str_Mod;
            else if (!FindWeapon(weapon).isMelee)
                modBouns = (FindWeapon(weapon).isFiness) ? ((Str_Mod > Dex_Mod) ? Str_Mod : Dex_Mod) : Dex_Mod;

            //check if the weapon is in the proficiency weapon list, then add proficiency bouns
            string weaponName = FindWeapon(weapon).name;
            bool isProficiencyWeapon = proficiencyWeaponList.Exists(o => o == weaponName);
            //Console.WriteLine(isProficiencyWeapon);

            int weaponBouns = (isProficiencyWeapon)? FindWeapon(weapon).bouns + proficiencyBouns: FindWeapon(weapon).bouns;
            int diceNumber = FindWeapon(weapon).diceNumber;
            int diceType = FindWeapon(weapon).diceType;

            int totalDamage = TotalDamage(times, statue, diceNumber, diceType, modBouns, weaponBouns, armorClass);
            double damageRate = Convert.ToDouble(totalDamage) / Convert.ToDouble(times);
            string testStatue = "";
            switch (statue)
            {
                case NAD.Normal:
                    testStatue = "Normal";
                    break;
                case NAD.Advantage:
                    testStatue = "Advantage";
                    break;
                case NAD.Disadvantage:
                    testStatue = "Disadvantage";
                    break;
            }
            Console.WriteLine("AC and Damage Check for: " + weapon + " vs AC" + armorClass + " with " + testStatue + " statue \n" +
                              "  - Test times:                      " + times + "\n" +
                              "  - AC Success Times:                " + attackSuccessTimes + "\n" +
                              "  - TotalDamage:                     " + totalDamage + "\n" +
                              "  - Damage Rate(Total Damage/Times): " + damageRate, 2);
        }
        public static double GetWeaponDamageRate(int times, NAD statue, string weapon, int ArmorClass)
        {
            int modBouns = 0;

            if (FindWeapon(weapon).isMelee)
                modBouns = (FindWeapon(weapon).isFiness) ? ((Str_Mod > Dex_Mod) ? Str_Mod : Dex_Mod) : Str_Mod;
            else if (!FindWeapon(weapon).isMelee)
                modBouns = (FindWeapon(weapon).isFiness) ? ((Str_Mod > Dex_Mod) ? Str_Mod : Dex_Mod) : Dex_Mod;

            //check if the weapon is in the proficiency weapon list, 
            string weaponName = FindWeapon(weapon).name;
            bool isProficiencyWeapon = proficiencyWeaponList.Exists(o => o == weaponName);
            Console.WriteLine(isProficiencyWeapon);

            //then add proficiency bouns
            int weaponBouns = (isProficiencyWeapon) ? FindWeapon(weapon).bouns + proficiencyBouns : FindWeapon(weapon).bouns;
           
            //weapon dice, number and type
            int diceNumber = FindWeapon(weapon).diceNumber;
            int diceType = FindWeapon(weapon).diceType;

            int totalDamage = TotalDamage(times, statue, diceNumber, diceType, modBouns, weaponBouns, ArmorClass);
            double damageRate = Convert.ToDouble(totalDamage) / Convert.ToDouble(times);
            return damageRate;
        }

        //------------------------------ability check------------------------------------------------------
        public static void AbilityCheck(int times, NAD Str, NAD Dex, NAD Con, NAD Int, NAD Wis, NAD Cha)
        {
            PrintAbilityCheck("Str", Str, times);
            PrintAbilityCheck("Dex", Dex, times);
            PrintAbilityCheck("Con", Con, times);
            PrintAbilityCheck("Int", Int, times);
            PrintAbilityCheck("Wis", Wis, times);
            PrintAbilityCheck("Cha", Cha, times);
        }
        public static double AbilityCheck(String t, int times, NAD nadstate, int difficultyLvl)
        {
            int veryEasySuccess = 0;            //5
            int easySuccess = 0;                //10
            int mediumSuccess = 0;              //15
            int hardSuccess = 0;                //20
            int veryHardSuccess = 0;            //25
            int nearlyImpossibleSuccess = 0;    //30

            int mod = 0;
            switch (t)
            {
                case "Str":
                    mod = Str_Mod;
                    break;
                case "Dex":
                    mod = Dex_Mod;
                    break;
                case "Con":
                    mod = Con_Mod;
                    break;
                case "Int":
                    mod = Int_Mod;
                    break;
                case "Wis":
                    mod = Wis_Mod;
                    break;
                case "Cha":
                    mod = Cha_Mod;
                    break;
            }

            for (int i = 0; i < times; i++)
            {
                int diceResult1 = RollDiceMethod2(20);
                System.Threading.Thread.Sleep(sleepTime);
                int diceResult2 = RollDiceMethod2(20);
                int diceResult = 0;

                switch (nadstate)
                {
                    case NAD.Normal:
                        diceResult = diceResult1;
                        break;
                    case NAD.Advantage:
                        diceResult = (diceResult1 > diceResult2) ? diceResult1 : diceResult2;
                        break;
                    case NAD.Disadvantage:
                        diceResult = (diceResult1 < diceResult2) ? diceResult1 : diceResult2;
                        break;
                }

                veryEasySuccess = ((diceResult + mod) > 5) ? veryEasySuccess + 1 : veryEasySuccess;
                easySuccess = ((diceResult + mod) > 10) ? easySuccess + 1 : easySuccess;
                mediumSuccess = ((diceResult + mod) > 15) ? mediumSuccess + 1 : mediumSuccess;
                hardSuccess = ((diceResult + mod) > 20) ? hardSuccess + 1 : hardSuccess;
                veryHardSuccess = ((diceResult + mod) > 25) ? veryHardSuccess + 1 : veryHardSuccess;
                nearlyImpossibleSuccess = ((diceResult + mod) > 30) ? nearlyImpossibleSuccess + 1 : nearlyImpossibleSuccess;
                System.Threading.Thread.Sleep(sleepTime);
            }

            if (difficultyLvl == 5)
                return Convert.ToDouble(veryEasySuccess) / Convert.ToDouble(times);
            if (difficultyLvl == 10)
                return Convert.ToDouble(easySuccess) / Convert.ToDouble(times);
            if (difficultyLvl == 15)
                return Convert.ToDouble(mediumSuccess) / Convert.ToDouble(times);
            if (difficultyLvl == 20)
                return Convert.ToDouble(hardSuccess) / Convert.ToDouble(times);
            if (difficultyLvl == 25)
                return Convert.ToDouble(veryHardSuccess) / Convert.ToDouble(times);
            if (difficultyLvl == 30)
                return Convert.ToDouble(nearlyImpossibleSuccess) / Convert.ToDouble(times);
            else
                return 0.00;
        }        
        public static double SkillCheck(int skill, int times, NAD nadstate)
        {
            int veryEasySuccess = 0;            //5
            int easySuccess = 0;                //10
            int mediumSuccess = 0;              //15
            int hardSuccess = 0;                //20
            int veryHardSuccess = 0;            //25
            int nearlyImpossibleSuccess = 0;    //30

            for (int i = 0; i < times; i++)
            {
                int diceResult1 = RollDiceMethod2(20);
                System.Threading.Thread.Sleep(sleepTime);
                int diceResult2 = RollDiceMethod2(20);
                int diceResult = 0;

                switch (nadstate)
                {
                    case NAD.Normal:
                        diceResult = diceResult1;
                        break;
                    case NAD.Advantage:
                        diceResult = (diceResult1 > diceResult2) ? diceResult1 : diceResult2;
                        break;
                    case NAD.Disadvantage:
                        diceResult = (diceResult1 < diceResult2) ? diceResult1 : diceResult2;
                        break;
                }

                veryEasySuccess = ((diceResult + skill) > 5) ? veryEasySuccess + 1 : veryEasySuccess;
                easySuccess = ((diceResult + skill) > 10) ? easySuccess + 1 : easySuccess;
                mediumSuccess = ((diceResult + skill) > 15) ? mediumSuccess + 1 : mediumSuccess;
                hardSuccess = ((diceResult + skill) > 20) ? hardSuccess + 1 : hardSuccess;
                veryHardSuccess = ((diceResult + skill) > 25) ? veryHardSuccess + 1 : veryHardSuccess;
                nearlyImpossibleSuccess = ((diceResult + skill) > 30) ? nearlyImpossibleSuccess + 1 : nearlyImpossibleSuccess;
            }

            double result = (Convert.ToDouble(veryEasySuccess) / Convert.ToDouble(times)) * 12 +
                            (Convert.ToDouble(easySuccess) / Convert.ToDouble(times)) * 18 +
                            (Convert.ToDouble(mediumSuccess) / Convert.ToDouble(times)) * 30 +
                            (Convert.ToDouble(hardSuccess) / Convert.ToDouble(times)) * 18 +
                            (Convert.ToDouble(veryHardSuccess) / Convert.ToDouble(times)) * 12 +
                            (Convert.ToDouble(nearlyImpossibleSuccess) / Convert.ToDouble(times)) * 10;

            return result;
        }
        public static double SkillsCheck(double[] checkList, int times)
        {
            double totalScore = 0;

            for (int i = 0; i < SkillsList.Count; i++)
            {

                if (checkList[i] != 0)
                {
                    double result = 0;
                    result = SkillCheck(SkillsList[i], times * 3, NAD.Advantage) +
                             SkillCheck(SkillsList[i], times * 8, NAD.Normal) +
                             SkillCheck(SkillsList[i], times * 3, NAD.Disadvantage);

                    //Console.WriteLine(SkillsList[i] + " : " + result);
                    totalScore += result * checkList[i];

                }
                //Console.WriteLine(i + " finish");
                //Console.WriteLine("total score : " + totalScore + "\n");
            }
            //Console.WriteLine("\nall finish");
            //Console.WriteLine("total score : " + totalScore);
            return totalScore;
        }

        //------------------------------Dice and default----------------------------------------------------
        public static int RollDiceMethod2(int DiceSide)
        {
            int i = 0;
            Random rdm = new Random();
            if (DiceSide == 4 || DiceSide == 6 || DiceSide == 8 || DiceSide == 12 || DiceSide == 20)
                i = new Random(Guid.NewGuid().GetHashCode()).Next(1, DiceSide + 1);
            if (DiceSide == 10 || DiceSide == 100)
                i = new Random(Guid.NewGuid().GetHashCode()).Next(0, DiceSide);
            return i;
        }
        public static Weapon FindWeapon(string name)
        {
            foreach (Weapon wp in weapons)
            {
                if (wp.name == name)
                    return wp;
            }
            return new Weapon("", 0, 0, false, false, 0);
        }


        public static void AddWeapons()
        {
            //name   diceNumber    diceType    isMelee    isFiness

            //simple melee weapons
            weapons.Add(new Weapon("Club", 1, 4, true, false, 0));
            weapons.Add(new Weapon("Dagger", 1, 4, true, true, 0));
            weapons.Add(new Weapon("Greatclub", 1, 8, true, false, 0));
            weapons.Add(new Weapon("Handaxe", 1, 6, true, false, 0));
            weapons.Add(new Weapon("Javelin", 1, 6, true, false, 0));
            weapons.Add(new Weapon("Light hammer", 1, 4, true, false, 0));
            weapons.Add(new Weapon("Mace", 1, 6, true, false, 0));
            weapons.Add(new Weapon("Quarterstaff", 1, 6, true, false, 0));
            weapons.Add(new Weapon("Sickle", 1, 4, true, false, 0));
            weapons.Add(new Weapon("Spear", 1, 6, true, false, 0));

            //simple ranged weapons
            weapons.Add(new Weapon("Crossbow,light", 1, 8, false, false, 0));
            weapons.Add(new Weapon("Dart", 1, 4, false, true, 0));
            weapons.Add(new Weapon("Shortbow", 1, 6, false, false, 0));
            weapons.Add(new Weapon("Sling", 1, 4, false, false, 0));

            //Martial melee wapons
            weapons.Add(new Weapon("Battleaxe", 1, 8, true, false, 0));
            weapons.Add(new Weapon("Flail", 1, 8, true, false, 0));
            weapons.Add(new Weapon("Glavie", 1, 10, true, false, 0));
            weapons.Add(new Weapon("GreatAxe", 1, 12, true, false, 0));
            weapons.Add(new Weapon("Greatsword", 2, 6, true, false, 0));
            weapons.Add(new Weapon("Halberd", 1, 10, true, false, 0));
            weapons.Add(new Weapon("Lance", 1, 12, true, false, 0));
            weapons.Add(new Weapon("Longsword", 1, 8, true, false, 0));
            weapons.Add(new Weapon("Maul", 2, 6, true, false, 0));
            weapons.Add(new Weapon("Morningstar", 1, 8, true, false, 0));
            weapons.Add(new Weapon("Pike", 1, 10, true, false, 0));
            weapons.Add(new Weapon("Rapier", 1, 8, true, true, 0));
            weapons.Add(new Weapon("Scimitar", 1, 8, true, true, 0));
            weapons.Add(new Weapon("Shortword", 1, 6, true, true, 0));
            weapons.Add(new Weapon("Trident", 1, 6, true, false, 0));
            weapons.Add(new Weapon("War pick", 1, 8, true, false, 0));
            weapons.Add(new Weapon("Warhammer", 1, 8, true, false, 0));
            weapons.Add(new Weapon("Whip", 1, 4, true, true, 0));

            //Martial Ranged 
            weapons.Add(new Weapon("Blowgun", 0, 0, true, false, 1));
            weapons.Add(new Weapon("Crossbow,hand", 1, 6, true, false, 0));
            weapons.Add(new Weapon("Crossbow,heavy", 1, 10, true, false, 0));
            weapons.Add(new Weapon("Longbow", 1, 8, true, false, 0));
            weapons.Add(new Weapon("Net", 0, 0, true, false, 0));
            //range weapon
        }

        public static void AddEnemies()
        {
            //                    |        Name      | AC |     Hit point      |            Ability                |   EnemyAttack >>[     Melee Attack   ][   Ranged Attack  ][MOD]|      
            enemies.Add(new Enemy("Bat"              , 12 , new Dice("1D4-1")  ,new Ability( 2, 15,  8, 2, 12, 4), new EnemyAttack (new Dice("null")  ,0, new Dice("null")  ,0, 1) ));
            enemies.Add(new Enemy("Black Bear"       , 11 , new Dice("3D8+6")  ,new Ability(15, 10, 14, 2, 12, 7), new EnemyAttack (new Dice("1D6+2") ,3, new Dice("2D4+2") ,3, 0) ));
            enemies.Add(new Enemy("Boar"             , 11 , new Dice("2D8+2")  ,new Ability(13, 11, 12, 2,  9, 5), new EnemyAttack (new Dice("1D6+1") ,3, new Dice("null")  ,3, 0) ));
            enemies.Add(new Enemy("Brown Bear"       , 11 , new Dice("4D10+12"),new Ability(19, 10, 16, 2, 13, 7), new EnemyAttack (new Dice("1D8+4") ,5, new Dice("2D6+4") ,5, 0) ));
            enemies.Add(new Enemy("Cat"              , 12 , new Dice("1D4+0")  ,new Ability( 3, 15, 10, 3, 12, 7), new EnemyAttack (new Dice("null")  ,0, new Dice("null")  ,0, 1) ));
            enemies.Add(new Enemy("Constrictor Snake", 12 , new Dice("2D10+2") ,new Ability(15, 14, 12, 1, 10, 3), new EnemyAttack (new Dice("1D6+2") ,4, new Dice("1D8+2") ,4, 0) ));
            enemies.Add(new Enemy("Crocodile"        , 12 , new Dice("3D10+3") ,new Ability(15, 10, 13, 2, 10, 5), new EnemyAttack (new Dice("1D10+2"),4, new Dice("null")  ,0, 0) ));
            enemies.Add(new Enemy("Dire Wolf"        , 14 , new Dice("5D10+10"),new Ability(17, 15, 15, 3, 12, 7), new EnemyAttack (new Dice("2D6+3") ,5, new Dice("null")  ,0, 0) ));
            enemies.Add(new Enemy("Frog"             , 11,  new Dice("1D4-1")  ,new Ability( 1, 13,  8, 1,  8, 3), new EnemyAttack (new Dice("null")  ,0, new Dice("null")  ,0, 0) ));
        }

        //public static int RollDice(int DiceSide)
        //{
        //    int i = 0;
        //    Random rdm = new Random();
        //    if (DiceSide == 4 || DiceSide == 6 || DiceSide == 8 || DiceSide == 12 || DiceSide == 20)
        //        i = rdm.Next(1, DiceSide + 1);
        //    if (DiceSide == 10 || DiceSide == 100)
        //        i = rdm.Next(0, DiceSide);
        //    return i;
        //}

        //for weapons


    }

    class Ability
    {
        public int STR, DEX, CON, INT, WIS, CHA;        
        public Ability(int STR, int DEX, int CON, int INT, int WIS, int CHA)
        {
            this.STR = STR;
            this.DEX = DEX;
            this.CON = CON;
            this.INT = INT;
            this.WIS = WIS;
            this.CHA = CHA;
        }        
    }

    class AbilityMod
    {
        public int STR, DEX, CON, INT, WIS, CHA;
        public AbilityMod(Ability ab)
        {
            this.STR = (ab.STR > 10) ? (int)Math.Floor((ab.STR - 10) / 2 + 0f) : (int)Math.Floor((ab.STR - 1 - 10) / 2 + 0f);
            this.DEX = (ab.DEX > 10) ? (int)Math.Floor((ab.DEX - 10) / 2 + 0f) : (int)Math.Floor((ab.DEX - 1 - 10) / 2 + 0f);
            this.CON = (ab.CON > 10) ? (int)Math.Floor((ab.CON - 10) / 2 + 0f) : (int)Math.Floor((ab.CON - 1 - 10) / 2 + 0f);
            this.INT = (ab.INT > 10) ? (int)Math.Floor((ab.INT - 10) / 2 + 0f) : (int)Math.Floor((ab.INT - 1 - 10) / 2 + 0f);
            this.WIS = (ab.WIS > 10) ? (int)Math.Floor((ab.WIS - 10) / 2 + 0f) : (int)Math.Floor((ab.WIS - 1 - 10) / 2 + 0f);
            this.CHA = (ab.CHA > 10) ? (int)Math.Floor((ab.CHA - 10) / 2 + 0f) : (int)Math.Floor((ab.CHA - 1 - 10) / 2 + 0f);
        }
    }

    class Weapon
    {
        public string name;
        public int diceNumber;
        public int diceType;
        public bool isMelee;
        public bool isFiness;
        public int bouns;

        public Weapon(string name, int diceNumber, int diceType, bool isMelee,bool isFiness,int bouns)
        {
            this.name = name;
            this.diceNumber = diceNumber;
            this.diceType = diceType;
            this.isMelee = isMelee;
            this.isFiness = isFiness;
            this.bouns = bouns;
        }       
    }

    class Dice
    {
        public int number;
        public int type;
        public int modify;

        public Dice(string nDri)
        {
            if (nDri == "null" || nDri == "Null")
            {
                this.number = 0;
                this.type = 0;
                this.modify = 0;
            }
            else if (nDri.IndexOf("+") >= 0)
            {
                string[] templStringArray = nDri.Split(new char[2] { 'D', '+'});
                this.number = int.Parse(templStringArray[0]);
                this.type = int.Parse(templStringArray[1]);
                this.modify = int.Parse(templStringArray[2]);
            }
            else if (nDri.IndexOf("-") >= 0)
            {
                string[] templStringArray = nDri.Split(new char[2] { 'D', '-' });
                this.number = int.Parse(templStringArray[0]);
                this.type = - int.Parse(templStringArray[1]);
                this.modify = int.Parse(templStringArray[2]);
            }
            else
            {
                string[] templStringArray = nDri.Split(new char[1] { 'D'});
                this.number = int.Parse(templStringArray[0]);
                this.type = int.Parse(templStringArray[1]);
                this.modify = 0;
            }
        }
    }   

    class EnemyAttack
    {
        public Dice meleeAttack;
        public int meleeAttackHitModify;
        public Dice rangedAttack;
        public int rangedAttackHitModify;
        public int modify;

        public EnemyAttack(Dice meleeAttack,int meleeAttackHitModify, Dice rangedAttack,int rangedAttackHitModify, int modify)
        {
            this.meleeAttack = meleeAttack;
            this.meleeAttackHitModify = meleeAttackHitModify;
            this.rangedAttack = rangedAttack;
            this.rangedAttackHitModify = rangedAttackHitModify;
            this.modify = modify;
        }
    }

    class Player
    {
        public string pRace, pClass;

        public Dice hitDice;

        public Ability ab;
        public AbilityMod abm;
        public Skills skills;

        public int proficiencyBouns;
        public List<string> proficiencyWeaponList = new List<string> { };
        public int armorClass;        

        public Player (string pRace, string pClass, Dice hitDice, Ability ab, int proficiencyBouns, int armorClass)
        {
            this.pRace = pRace;
            this.pClass = pClass;

            this.hitDice = hitDice;

            this.ab = ab;
            this.abm = new AbilityMod(ab);
            this.skills = new Skills(abm);

            this.proficiencyBouns = proficiencyBouns;
            this.proficiencyWeaponList = new List<string>();
            this.armorClass = armorClass;            
        }

       

        public void ExtraAbility(int StrNum, int DexNum, int ConNum, int IntNum, int WisNum, int ChaNum)
        {
            ab.STR += StrNum;
            ab.DEX += DexNum;
            ab.CON += ConNum;
            ab.INT += IntNum;
            ab.WIS += WisNum;
            ab.CHA += ChaNum;
        }

        public void AddProficiencyBouns(int newNumber)
        {
            this.proficiencyBouns += newNumber;
        }

        public void AddProficiencyWeapon(string[] list)
        {
            int length = list.Length;
            for (int i = 0; i < length; i++)
            {
                this.proficiencyWeaponList.Add(list[i]);
            }
        }

        public void ClearProficiencyWeapon()
        {
            this.proficiencyWeaponList.Clear();
        }

        void UpdateData()
        {
            this.abm = new AbilityMod(ab);
            this.skills = new Skills(abm);
        }
    }   


    class Skills
    {
        public int Athletics,SleightofHand, Stealth, Arcana, History, Nature, Religion, Investigation, AnimalHandling, Insight, Medicine, Perception, Survival, Deception, Intimidation, Performance, Persuasion;

        public Skills(AbilityMod abm)
        {
            //Str
            this.Athletics       = abm.STR;

            //Dex
            this.SleightofHand   = abm.DEX;
            this.Stealth         = abm.DEX;

            //Con

            //Int
            this.Arcana          = abm.INT;
            this.History         = abm.INT;
            this.Nature          = abm.INT;
            this.Religion        = abm.INT;
            this.Investigation   = abm.INT;

            //Wis
            this.AnimalHandling  = abm.WIS;
            this.Insight         = abm.WIS;
            this.Medicine        = abm.WIS;
            this.Perception      = abm.WIS;
            this.Survival        = abm.WIS;

            //Cha
            this.Deception       = abm.CHA;
            this.Intimidation    = abm.CHA;
            this.Performance     = abm.CHA;
            this.Persuasion      = abm.CHA;
        }
    }

    class Enemy
    {
        public string name;
        public int armorClass;
        public Dice hitPoint;

        public Ability eab;
        //public static int Stre, Dexe, Cone, Inte, Wise, Chae;
        //public static int Str_Mode, Dex_Mode, Con_Mode, Int_Mode, Wis_Mode, Cha_Mode;
        //public Weapon wp;
        public EnemyAttack ea;

        public Enemy(string name,int armorClass, Dice hitPoint, Ability eab , EnemyAttack ea)
        {
            this.name = name;
            this.armorClass = armorClass;
            this.hitPoint = hitPoint;
            this.eab = eab;
            this.ea = ea;
        }       
    }
}
