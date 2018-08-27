using System;
using System.Collections;
using System.Collections.Generic;

namespace DnDv22
{
    class MainClass
    {
        //                                                   杂技 体育 骗人的戏法  隐身 奥秘 历史  自然  宗教 调查 动物处理 洞察 医学 知觉 生存 欺骗 恐吓 表演 说服 
        static double[] ExplorationSkillsInfluence =       { 0.5, 1, 0, 0.8, 0.8, 0.2, 1, 0.2, 0.8, 0.9, 0.1, 0.6, 0.2, 1, 0, 0, 0, 0, };
        static double[] SocialInteractionSkillsInfluence = { 0.5, 0, 1, 0.2, 0.2, 0.8, 0, 0.8, 0.2, 0.1, 0.9, 0.6, 0.8, 0, 1, 1, 1, 1, };

        public static void Main(string[] args)
        {

            // RoolDiceCheck("1D10+0", 100);   

            //initial
            EquipmentBase eb = new EquipmentBase();
            UnitBase ub = new UnitBase(eb);

            //Player player = ub.players[0];

            Console.WriteLine(ub.players.Count);

            ////------------------------------------test start---------------------------------------------
            for (int i = 0; i < ub.players.Count; i++)
            {
                Console.WriteLine("\n"+i);
                Test(ub.players[i], ub);
            }
            
            Console.ReadLine();
        }
        


        public static void Test(Player player, UnitBase ub)
        {
            double result = 0;

            ////1. exploration
            double exploratiionResult = 0;
            for (int i = 0; i < 1000; i++)
            {
                Player copy = player.GetCopy();
                exploratiionResult += copy.skills.SkillsCheck(ExplorationSkillsInfluence, 1, copy);
            }
            exploratiionResult = exploratiionResult / 300;
            result += exploratiionResult;
            Console.WriteLine(player.GetClass() + " + " + player.GetRace() + "\n1. Exploration test result: \n" + exploratiionResult);

            ////2. social interaction
            double socialInteractionResult = 0;
            for (int j = 0; j < 1000; j++)
            {
                Player copy = player.GetCopy();
                socialInteractionResult += copy.skills.SkillsCheck(SocialInteractionSkillsInfluence, 1, copy);
            }
            socialInteractionResult = socialInteractionResult / 300;
            result += socialInteractionResult;
            Console.WriteLine("2. Social interaction test  result: \n" + socialInteractionResult);

            //3.combat
            double combatResult = BattleCheck(1000, player, ub.enemies);
            result += combatResult;
            Console.WriteLine("3. Combat test result: \n" + combatResult);

            Console.WriteLine("\nFinal result: \n" + result);
            ////------------------------------------test finish---------------------------------------------
        }

        public static void RoolDiceCheck(String nDri, int times)
        {
            /////////////////////////////////////////////////////////
            //                  Roll dice Check                    //
            int result = 0;                                        //
            for (int i = 0; i < times; i++)                        //
            {                                                      //
                result = Dice.RollDice(nDri);                      //
                Console.WriteLine(nDri + " result: " + result);    //
            }                                                      //
            ///////////////////////////////////////////////////////// 
        }

        ////-------------------------------combat check------------------------------------------------------      
        // Simulate Battle
        public static int BattleCheck(int times, Player p, List<Enemy> eList)
        {
            int result = 0;

            foreach (Enemy e in eList)
            {
                result += BattleCheck(times, p, e);
            }
            return result;
        }
        public static int BattleCheck(int minTime, int maxTime, Player p, Enemy e)
        {
            int times = new Random(Guid.NewGuid().GetHashCode()).Next(minTime, maxTime);
            return BattleCheck(times,p,e);
        }
        public static int BattleCheck(int times, Player p, Enemy e)
        {
            int result = 0;
            for (int t = 0; t < times; t++)
            {
                Player pCopy = p.GetCopy();
                Enemy eCopy = e.GetCopy();
                //Console.WriteLine("\n" + p.health + "    " + e.health);
                //Console.WriteLine("\n" + pCopy.health + "    " + eCopy.health);                
                result += SimulateBattle(pCopy, eCopy);
                //Console.WriteLine(p.health + "    " + e.health);
                //Console.WriteLine(pCopy.health + "    " + eCopy.health);
            }
            return result;
        }
        // Player VS Enemy, || return 1 player win || return 0 enemy win
        public static int SimulateBattle(Player p, Enemy e)
        {
            p.nad = Status.NAD.Normal;
            e.nad = Status.NAD.Normal;

            int result = 0;

            //decide player move first or enemy move first 
            int pDice = 0, eDice = 0;
            while (pDice == eDice)
            {
                pDice = Dice.RollDice("1D10 + 0");
                eDice = Dice.RollDice("1D10 + 0");
            }

            //for (int i = 0; i < 100; i++)
            //{
            //if (eDice > pDice)
            //    Console.WriteLine("enemy move first");

            //handle Extral Capacity
            bool isRelentlessEnduranceUsed = true;
            if (p.ExtrallCapacity.Contains("RelentlessEndurance"))
            {
                isRelentlessEnduranceUsed = false;
            }
            for (int c = 0; c < 40; c++)
            {
                if (p.health > 0 && e.health > 0)
                {
                    if (pDice > eDice)//player move first
                    {
                        Combat(p, e);
                        if (e.health > 0)
                            Combat(e, p);
                    }

                    if (pDice < eDice)//enemy move first
                    {
                        Combat(e, p);
                        if (p.health > 0)
                            Combat(p, e);
                    }

                    if (isRelentlessEnduranceUsed == false && p.health < 1)
                    {
                        p.health = 1;
                        isRelentlessEnduranceUsed = true;
                    }
                }
            }
            //}

            result = (e.health <= 0)? 1: 0;
            
            return result;
        }
        //----------------------- PVE ------------------------------       
        // Player VS Enemy, || player attack, enemy defense
        public static void Combat(Player p, Enemy e)  
        {
            int modBouns = 0;
            Weapon weapon = p.GetWeapon(); 

            //check 1)if the weapon si a finess weapon 2) add ability modify bouns base on the weapon type
            if (weapon.range == Weapon.Range.Melee)
                modBouns = (weapon.isFiness) ? ((p.abm.STR > p.abm.DEX) ? p.abm.STR : p.abm.DEX) : p.abm.STR;
            else if (weapon.range == Weapon.Range.Ranged)
                modBouns = (weapon.isFiness) ? ((p.abm.STR > p.abm.DEX) ? p.abm.STR : p.abm.DEX) : p.abm.DEX;

            //check 1)if the weapon is in the proficiency weapon list 2) add proficiency bouns
            bool isProficiencyWeapon = p.proficiencyWeaponList.Exists(o => o == weapon.name);
            int proficiencyBouns = (isProficiencyWeapon) ? p.proficiencyBouns : 0;

            //total Bouns
            int AttackBouns = proficiencyBouns + modBouns;

            //the weapon dice
            Dice playerWeaponDice = weapon.damageDice;

            //check 1)is attack success 2)damage             
            AttackChecker acer = AttackCheck(p, AttackBouns, e.armorClass);
            if (acer.isAttackSuccess)
                e.health -= DamageCheck(p, weapon, acer.isCritical);   
        }
        // check if the attack 1)success or not, 2)is cretic or not
        public static AttackChecker AttackCheck(Player p, int attackBouns, int armorClass)
        {
            bool D20rerolled = true;

            if (p.ExtrallCapacity.Contains("RerollD20"))
            {
                D20rerolled = false;
            }

            rd:
            int d20Result = 0;
            int d20Result2 = 0;
            int attack = 0;

            Status.NAD AttackerNAD = p.nad;

            d20Result = Dice.RollDice("1D20+0");
            d20Result2 = Dice.RollDice("1D20+0");

             //attack check > armor class   => success
            //attack check == 20           =? critical
            //attack check == 1            => always fail
            bool isCritSuc = false;
            bool isFail = false;
            bool isAttackSuccess;
            switch (AttackerNAD)
            {
                case Status.NAD.Normal://Normal                    
                    attack = d20Result + attackBouns;
                    isFail = (attack == 1);
                    isCritSuc = (attack == 20);
                    isAttackSuccess  = (isFail) ? false : (attack > (armorClass - 1));

                    if (D20rerolled == false && isAttackSuccess == false)
                    {
                        D20rerolled = true;
                        goto rd;
                    }

                    return new AttackChecker(isAttackSuccess,isCritSuc);

                case Status.NAD.Advantage: //Advantage
                    attack = (d20Result > d20Result2) ? d20Result + attackBouns : d20Result2 + attackBouns;
                    isCritSuc = (d20Result > d20Result2) ? (d20Result == 20) : (d20Result2 == 20);
                    isFail = (d20Result > d20Result2) ? (d20Result == 1) : (d20Result2 == 1);
                    isAttackSuccess = (isFail) ? false : (attack > (armorClass - 1));

                    if (D20rerolled == false && isAttackSuccess == false)
                    {
                        D20rerolled = true;
                        goto rd;
                    }

                    return new AttackChecker(isAttackSuccess,isCritSuc);  
                    
                case Status.NAD.Disadvantage: //Disadvantage
                    attack = (d20Result < d20Result2) ? d20Result + attackBouns : d20Result2 + attackBouns;
                    isCritSuc = (d20Result < d20Result2) ? (d20Result == 20) : (d20Result2 == 20);
                    isFail = (d20Result < d20Result2) ? (d20Result == 1) : (d20Result2 == 1);
                    isAttackSuccess = (isFail) ? false : (attack > (armorClass - 1));

                    if (D20rerolled == false && isAttackSuccess == false)
                    {
                        D20rerolled = true;
                        goto rd;
                    }

                    return new AttackChecker(isAttackSuccess,isCritSuc);
            }
            return new AttackChecker(false, false);           
        }
        // check 3) how much damage it deal
        // reference : https://rpg.stackexchange.com/questions/72910/how-do-i-figure-the-dice-and-bonuses-for-attack-rolls-and-damage-rolls
        // damage = weapon hitdice + mod
        public static int DamageCheck(Player p, Weapon wp, bool isCritical)
        {
            int damage = 0;

            //check 1)if the weapon si a finess weapon 2) add ability modify bouns base on the weapon type
            int modBouns = 0;
            if (wp.range == Weapon.Range.Melee)
                modBouns = (wp.isFiness) ? ((p.abm.STR > p.abm.DEX) ? p.abm.STR : p.abm.DEX) : p.abm.STR;
            else if (wp.range == Weapon.Range.Ranged)
                modBouns = (wp.isFiness) ? ((p.abm.STR > p.abm.DEX) ? p.abm.STR : p.abm.DEX) : p.abm.DEX;

            //check 3) weapon damage
            int weaponDamage = Dice.RollDice(wp.damageDice.GetDice());

            //check 4)if critical 5)critical damage
            int criticalDamage = (isCritical ? Dice.RollDice(wp.damageDice.GetDice()) : 0);
            //extralCriticalDamage
            bool isSavegeAttacksWork = p.ExtrallCapacity.Contains("SavegeAttacks");
            bool isUsingMelleWeapon = ((wp.type == Weapon.Type.MartialMeleeWeapon) || (wp.type == Weapon.Type.SimpleMeleeWeapon));
            int extralCriticalDamage = 0;
            if (isSavegeAttacksWork && isUsingMelleWeapon)
                extralCriticalDamage = Dice.RollDice(wp.damageDice.GetDice());


            //check 6)total damage
            damage = weaponDamage + criticalDamage + extralCriticalDamage + modBouns;

            return (damage > 0) ? damage : 0;
        }
        //----------------------------------------------------------  
        //----------------------- EVP ------------------------------ 
        // Enemy VS Player, || enemy attack, player defense
        public static void Combat(Enemy e, Player p )  
        {
            //TODO 1)enemy attack and danmage calculate

            //check 1)is attack success 2)damage             
            AttackChecker acer = AttackCheck(e,p);
            if (acer.isAttackSuccess)
                p.health -= DamageCheck(e,acer.isCritical);
        }       
        // check if the attack 1)success or not, 2)is critic or not
        public static AttackChecker AttackCheck(Enemy e, Player p)
        {
            int armorClass = p.armorClass;
            bool isEnemyMelee = (e.ea.meleeAttack.r != 0 || e.ea.meleeAttack.i != 0);
            bool isEnemyRanged = (e.ea.rangedAttack.r != 0 || e.ea.rangedAttack.i != 0);

            int bouns = 0;
            int meleeMod = 0;
            int RangedMod = 0;
            int enemyBouns = e.ea.meleeAttackHitModify;
            if (isEnemyMelee && isEnemyRanged)
            {
                meleeMod = e.abm.STR;
                RangedMod = e.abm.DEX;
                bouns = (meleeMod > RangedMod) ? meleeMod : RangedMod;
                bouns += enemyBouns;
            }
            else if(isEnemyMelee)
            {
                bouns = e.abm.STR;
                bouns += enemyBouns;
            }
            else if(isEnemyRanged)
            {
                bouns = e.abm.DEX;   
                bouns += enemyBouns;
            }


            int d20Result = 0;
            int d20Result2 = 0;
            int attack = 0;


            Status.NAD AttackerNAD = p.nad;

            d20Result = Dice.RollDice("1D20+0");
            d20Result2 = Dice.RollDice("1D20+0");

            //attack check > armor class   => success
            //attack check == 20           =? critical
            //attack check == 1            => always fail
            bool isCritSuc = false;
            bool isFail = false;
            switch (AttackerNAD)
            {
                case Status.NAD.Normal://Normal                    
                    attack = d20Result + bouns;
                    isFail = (d20Result == 1);
                    return new AttackChecker( 
                                                (isFail) ? false : (attack >(armorClass - 1)),
                                                (d20Result == 20));
                case Status.NAD.Advantage: //Advantage
                    attack = (d20Result > d20Result2) ? d20Result + bouns : d20Result2 + bouns;
                    isCritSuc = (d20Result > d20Result2) ? (d20Result == 20) : (d20Result2 == 20);
                    isFail = (d20Result > d20Result2) ? (d20Result == 1) : (d20Result2 == 1);
                    return new AttackChecker( 
                                                (isFail)? false: (attack > (armorClass - 1)),
                                                isCritSuc);
                case Status.NAD.Disadvantage: //Disadvantage
                    attack = (d20Result < d20Result2) ? d20Result + bouns : d20Result2 + bouns;
                    isCritSuc = (d20Result < d20Result2) ? (d20Result == 20) : (d20Result2 == 20);
                    isFail = (d20Result < d20Result2) ? (d20Result == 1) : (d20Result2 == 1);
                    return new AttackChecker(
                                                (isFail) ? false : (attack > (armorClass - 1)),
                                                isCritSuc);
            }
            return new AttackChecker(false, false);
        }
        // check 3) how much damage it deal
        // reference : https://rpg.stackexchange.com/questions/72910/how-do-i-figure-the-dice-and-bonuses-for-attack-rolls-and-damage-rolls
        // damage = weapon hitdice + mod
        public static int DamageCheck(Enemy e, bool isCritical)
        {
            int damage = 0;

            // handle melee weapon damage
            int meleeDamage = 0;
            if (e.ea.meleeAttack.i != 0 || e.ea.meleeAttack.n != 0 || e.ea.meleeAttack.r != 0)
                meleeDamage = Dice.RollDice(e.ea.meleeAttack.GetDice()) + e.abm.STR;

            meleeDamage = (meleeDamage > 0) ? meleeDamage : 0;

            // handle ranged weapon damage 
            int rangedDamage =0;
            if (e.ea.rangedAttack.i != 0 || e.ea.rangedAttack.n != 0||e.ea.rangedAttack.r != 0)
                rangedDamage = Dice.RollDice(e.ea.rangedAttack.GetDice()) + e.abm.DEX;

            rangedDamage = (rangedDamage > 0) ? rangedDamage : 0;

            //check 4)if critical 5)critical damage
            int criticalDamage = (isCritical ? (Dice.RollDice(e.ea.meleeAttack.GetDice()) + Dice.RollDice(e.ea.rangedAttack.GetDice())): 0);
            criticalDamage = (criticalDamage > 0) ? criticalDamage : 0;            

            //check 6)total damage
            damage = meleeDamage  + rangedDamage + criticalDamage;

            //Console.WriteLine(damage);
            return (damage > 0) ? damage : 0;
        }
        //----------------------------------------------------------   
        //------------------------------ default setting---------------------------------------------------- 
    }

    //base code
    class Dice
    {
        public int n;
        public int r;
        public int i;

        public Dice(string nDri)
        {
            if (nDri == null)
                nDri = "0D0+0";
            else if (nDri == "null" || nDri == "Null")
            {
                this.n = 0;
                this.r = 0;
                this.i = 0;
            }
            else if (nDri.IndexOf("+") >= 0)
            {
                string[] templStringArray = nDri.Split(new char[2] { 'D', '+' });
                this.n = int.Parse(templStringArray[0]);
                this.r = int.Parse(templStringArray[1]);
                this.i = int.Parse(templStringArray[2]);
            }
            else if (nDri.IndexOf("-") >= 0)
            {
                string[] templStringArray = nDri.Split(new char[2] { 'D', '-' });
                this.n = int.Parse(templStringArray[0]);
                this.r = -int.Parse(templStringArray[1]);
                this.i = int.Parse(templStringArray[2]);
            }
            else
            {
                string[] templStringArray = nDri.Split(new char[1] { 'D' });
                this.n = int.Parse(templStringArray[0]);
                this.r = int.Parse(templStringArray[1]);
                this.i = 0;
            }
        }

        public static int RollDice(string dice)
        {
            Dice nDri = new Dice(dice);

            int result = 0;
            for (int n = 0; n < nDri.n; n++)
            {
                Random rdm = new Random();
                if (nDri.r == 4 || nDri.r == 6 || nDri.r == 8 || nDri.r == 12 || nDri.r == 20)
                    result += new Random(Guid.NewGuid().GetHashCode()).Next(1, nDri.r + 1);
                if (nDri.r == 10 || nDri.r == 100)
                    result += new Random(Guid.NewGuid().GetHashCode()).Next(0, nDri.r);
            }

            result += nDri.i;
            return result;
        }

        public string GetDice()
        {
            if (i < 0)
                return (n + "D" + r + "-" + Math.Abs(i));
            else
                return (n + "D" + r + "+" + i);
        }
    }

    //unit | include player & enemy
    class Status
    {    
        public enum NAD
        {
            Normal,
            Advantage,
            Disadvantage
        }
    }
    class Unit : Status 
    {
        //public Dice hitDice;
        public int maxHealth;
        public int health;

        public Ability ab;
        public AbilityMod abm;

        //for battle
        public AttackChecker attackChecker;
        public int armorClass;        
        public NAD nad;        

        public Unit()
        {
            Ini();   
            this.abm = new AbilityMod(ab);
        }

        public void Ini()
        {
            this.ab = new Ability(0, 0, 0, 0, 0, 0);
        }
    } 
    //player
    class Player : Unit
    {
        public enum Race
        {
            Dwarf_HillDwarf,
            Dwarf_MountainDwarf,
            Elf_HighElf,
            Elf_WoddElf,
            Elf_DarkElf,
            Halfling_Lightfoot,
            Halfling_Stout,
            Human,
            Dragonborn,
            Gnome_ForestGnome,
            Gnome_RockGnome,
            HalfElf,
            HalfOrc,
            Tiefling

        }
        public enum Class
        {
            Barbarian_PathOfTheBerserker,
            Barbarian_PathOfTheWarrior,
            Bard_CollegeOfLore,
            Bard_CollegeOfValor,
            Cleric_KnowledgeDomain,
            Cleric_LifeDomain,
            Cleric_LightDomain,
            Cleric_NatureDomain,
            Cleric_TempestDomain,
            Cleric_TrickeryDomain,
            Cleric_War_Domain,
            Druid_CircleOfTheLand,
            Druid_CircleOfTHeMoon,
            Fighter_Champion,
            Fighter_BattleMaster,
            Fighter_EldritchKnight,
            Monk_WayOfTheOpenHand,
            Monk_WayOfShadow,
            Monk_WayOfFourElements,
            Paladin_OathOfDevotion,
            Paladin_OathOfAncients,
            Paladin_OathOfVengance,
            Ranger_Hunter,
            Ranger_BeastMaster,
            Rogue_Thief,
            Rogue_Assassin,
            Rogue_ArcaneTrickester,
            Sorcerer_DraconicBloodine,
            Sorcerer_WildMagic,
            Warlock_TheArchfey,
            Warlock_TheFiend,
            Warlock_ThwGreatOldOne,
            Wizard_SchoolOfAbjuration,
            Wizard_SchoolOfConjuration,
            Wizard_SchoolOfDivination,
            Wizard_SchoolOfEnchantment,
            Wizard_SchoolOfEvocation,
            Wizard_SchoolOfIllusion,
            Wizard_SchoolOfNecromancy,
            Wizard_SchoolOfTransmutation

        }

        public Race pRace;
        public Class pClass;

        //class decide player's primary ability
        public List<string> primaryAbility = new List<string>();

        //class decide player's hit dice
        public Dice hitDice = new Dice("0D0+0");       
        
        public Skills skills= new Skills(new AbilityMod(new Ability(10,10,10,10,10,10)));

        public int proficiencyBouns;
       

        EquipmentBase eb;
        //list contain weapon player proficiancy with
        public List<string> proficiencyWeaponList = new List<string> { };   //store weapon type player proficiency with
        public List<string> playerWeapons = new List<string>(); // store weapon name player hold    (string)
        public List<Weapon> weapons = new List<Weapon>();       // store weapon player hold         (weapon)
        public Weapon weapon;                                   // weapon player equip              (weapon)

        public List<string> proficiencyArmorList = new List<string> { };    //store armor type player proficiency with
        public List<string> playerArmors = new List<string>();  // store armor name player hold     (string)
        public List<Armor> armors = new List<Armor>();          // store armor player hold          (weapon)
        public Armor armor;                                     // armor player equip               (armor)

        public List<string> ExtrallCapacity = new List<string>() { };
        //1) "RerollD20": Reroll D20 when attack roll, Ability Check, or Saving Throw, once
        //2) "RelentlessEndurance" : When hit point is reduced to 0, back to 1 (CD: a long breack)
        public Player(Race pRace, Class pClass, EquipmentBase eb, int proficiencyBouns)
        {
            this.Ini();
            this.eb = eb;
            this.pRace = pRace;
            this.pClass = pClass;
            RaceModify(pRace);
            ClassModify(pClass);

            this.maxHealth = hitDice.r + abm.CON;//Dice.RollDice(hitDice.n + "D" + hitDice.r + "+" + hitDice.i) + abm.CON;
            this.health = maxHealth;

            //automatic set ability base on class primary ablity
            AutoSetAbility(primaryAbility);

            //calculate ability and skills modify
            this.abm = new AbilityMod(ab);
            this.skills = new Skills(abm);

            this.proficiencyBouns = proficiencyBouns;

            //handle Weapons, Automatic add weapon by name
            foreach (string weaponName in playerWeapons)
            {
                this.AddWeapon(eb, weaponName);
            }
            weapon = ChoseWeapon(weapons);

            //handle Armors, Automatic add armor by name
            foreach (string armorName in playerArmors)
            {
                this.AddArmor(eb, armorName);
            }
            armor = ChoseArmor(armors);

            if (this.armorClass == 0 && armor != null)            
                this.armorClass = armor.GetAC(this);
            
        }
        void Ini()
        {
            this.ab = new Ability(0, 0, 0, 0, 0, 0);
            this.abm = new AbilityMod(ab);
            this.skills.AnimalHandling = 0;
            this.skills.Athletics = 0;
            this.skills.Deception = 0;
            this.skills.History = 0;
            this.skills.Insight = 0;
            this.skills.Intimidation = 0;
            this.skills.Investigation = 0;
            this.skills.Medicine = 0;
            this.skills.Nature = 0;
            this.skills.Perception = 0;
            this.skills.Persuasion = 0;
            this.skills.Religion = 0;
            this.skills.SleightofHand = 0;
            this.skills.Stealth = 0;
            this.skills.Survival = 0;
            hitDice = new Dice("1D0+0");
            maxHealth = 0;
            health = 0;
            this.armorClass = 0;
        }
        void UpdateData()
        {
            this.abm = new AbilityMod(ab);
            this.skills = new Skills(abm);
        }

        //chose a weapon from the list, randomly, could be update in the future to make it more artificial inteligent
        public Weapon ChoseWeapon(List<Weapon> weapons)
        {
            if (weapons != null)
                return weapons[new Random(Guid.NewGuid().GetHashCode()).Next(0, weapons.Count - 1)];
            else return new Weapon("null", Weapon.Type.Null,  new Dice("0D0 + 0"), Weapon.Range.Null, false);
        }
        //chose a Armor from the list, randomly, could be update in the future to make it more artificial inteligent
        public Armor ChoseArmor(List<Armor> armors)
        {
            if (armors.Count > 0)
                return armors[new Random(Guid.NewGuid().GetHashCode()).Next(0, armors.Count)];
            else
                return null;
        }
         
        public void RaceModify(Race race)
        {
            switch (race)
            {
                case Race.Dwarf_HillDwarf:
                    ab.CON += 2;
                    ab.WIS += 1;
                    hitDice.n += 1;
                    break;
                case Race.Dwarf_MountainDwarf:
                    ab.CON += 2;
                    ab.STR += 2;
                    ProficiencyArmorAdd(new List<Armor.Type>{ Armor.Type.LightArmor,Armor.Type.MediumArmor });
                    break;
                case Race.Elf_HighElf:
                    ab.DEX += 2;
                    ab.INT += 1;
                    proficiencyWeaponList.Add("Longsword");
                    proficiencyWeaponList.Add("Shortsword");
                    proficiencyWeaponList.Add("Shortbow");
                    proficiencyWeaponList.Add("Longbow");
                    break;
                case Race.Elf_WoddElf:
                    ab.DEX += 2;
                    ab.WIS += 1;
                    proficiencyWeaponList.Add("Longsword");
                    proficiencyWeaponList.Add("Shortsword");
                    proficiencyWeaponList.Add("Shortbow");
                    proficiencyWeaponList.Add("Longbow");
                    break;
                case Race.Elf_DarkElf:
                    ab.DEX += 2;
                    ab.CHA += 1;
                    proficiencyWeaponList.Add("Rapier");
                    proficiencyWeaponList.Add("Shortsword");
                    proficiencyWeaponList.Add("HandCrossbows");
                    break;
                case Race.Halfling_Lightfoot:
                    ab.DEX += 2;
                    ab.CHA += 1;
                    ExtrallCapacity.Add("RerollD20");
                    break;
                case Race.Halfling_Stout:
                    ab.DEX += 2;
                    ab.CON += 1;
                    ExtrallCapacity.Add("RerollD20");
                    break;
                case Race.Human:
                    ab.STR += 1;
                    ab.DEX += 1;
                    ab.CON += 1;
                    ab.INT += 1;
                    ab.WIS += 1;
                    ab.CHA += 1;
                    break;
                case Race.Dragonborn:
                    ab.STR += 2;
                    ab.CHA += 1;
                    break;
                case Race.Gnome_ForestGnome:
                    ab.INT += 2;
                    ab.DEX += 1;
                    break;
                case Race.Gnome_RockGnome:
                    ab.INT += 2;
                    ab.CON += 1;
                    break;
                case Race.HalfElf:
                    ab.CHA += 2;
                    ExtralAbility(2, new List<string> { "STR","DEX","CON","INT","WIS" });
                    break;
                case Race.HalfOrc:
                    ab.STR += 2;
                    ab.CON += 1;
                    ExtrallCapacity.Add("RelentlessEndurance");// When hit point is reduced to 0, back to 1 (CD: a long breack)
                    ExtrallCapacity.Add("SavegeAttacks"); // When critical hit with melee weapon, roll the weapon damage dice and add it to the extral damage
                    break;
                case Race.Tiefling:
                    ab.INT += 1;
                    ab.CHA += 2;
                    break;
            }
        }
        public string GetRace()
        {
            switch (this.pRace)
            {
                case Race.Dragonborn:
                    return "Dragonborn";
                case Race.Dwarf_HillDwarf:
                    return "Dwarf: Hill Dwarf";
                case Race.Dwarf_MountainDwarf:
                    return "Dwarf: Mountain Dwarf";
                case Race.Elf_DarkElf:
                    return "Elf: Dark Elf";
                case Race.Elf_HighElf:
                    return "Elf: High Elf";
                case Race.Elf_WoddElf:
                    return "Elf: Wodd Elf";
                case Race.Gnome_ForestGnome:
                    return "Gnome: Forest Gnome";
                case Race.Gnome_RockGnome:
                    return "Gnome: Rock Gnome";
                case Race.HalfElf:
                    return "Half-Elf";
                case Race.Halfling_Lightfoot:
                    return "Halfling: Lightfoot";
                case Race.Halfling_Stout:
                    return "Halfling: Stout";
                case Race.HalfOrc:
                    return "Half-Orc";
                case Race.Human:
                    return "Human";
                case Race.Tiefling:
                    return "Tiefling";
            }
            return null;
        }
        public void ClassModify(Class c)
        {
            switch (c)
            {
                //For Barbarian
                case Class.Barbarian_PathOfTheBerserker:
                case Class.Barbarian_PathOfTheWarrior:
                    hitDice.r += 12;
                    primaryAbility.Add("STR");
                    PlayerWeanponsAdd(new List<string> { "GreatAxe", "Handaxe", "Javelin"});
                    PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.MartialMeleeWeapon, Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon });
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.MediumArmor, Armor.Type.LightArmor, Armor.Type.Shield });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon,Weapon.Type.SimpleRangedWeapon,Weapon.Type.MartialMeleeWeapon, Weapon.Type.MartialRangedWeapon } );
                    ExtralSkill(2,new List<string>{ "AnimalHandling", "Athletics", "Intimidation", "Nature", "Perception", "Survival"});
                    if (armor == null)
                    {
                        this.armorClass = 10 + abm.DEX + abm.CON; 
                    }
                    break;

                //For Bard
                case Class.Bard_CollegeOfLore:
                case Class.Bard_CollegeOfValor:                    
                    hitDice.r = 8;
                    primaryAbility.Add("CHA");
                    PlayerWeanponsAdd(new List<string> { "Rapier", "Longsword", "Dagger" });
                    PlayerWeanponsAdd(new List<Weapon.Type> {Weapon.Type.SimpleRangedWeapon, Weapon.Type.SimpleMeleeWeapon});
                    PlayerArmorsAdd(new List<string> { "Leather"});
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon });
                    ProficiencyWeaponAdd(new List<string> { "Crossbow,hand", "Longsword", "Rapier", "Shortsword"});
                    ExtralSkill(3, new List<string> { "Athletics", "SleightofHand", "Stealth", "Arcana", "History", "Nature", "Religion", "Investigation", "AnimalHandling", "Insight", "Medicine", "Perception", "Survival", "Deception", "Intimidation", "Performance", "Persuasion" });
                    break;

                //For Cleric
                case Class.Cleric_LifeDomain:
                case Class.Cleric_War_Domain:
                case Class.Cleric_LightDomain:
                case Class.Cleric_NatureDomain:
                case Class.Cleric_TempestDomain:
                case Class.Cleric_TrickeryDomain:
                case Class.Cleric_KnowledgeDomain:
                    hitDice.r = 8;
                    primaryAbility.Add("WIS");
                    PlayerWeanponsAdd(new List<string> { "Mace", "Warhammer", "Crossbow,light"});
                    PlayerWeanponsAdd(new List<Weapon.Type> {Weapon.Type.SimpleRangedWeapon, Weapon.Type.SimpleMeleeWeapon });
                    PlayerArmorsAdd(new List<string> { "ScaleMail", "Leather", "ChainMail", "Shield"});
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor, Armor.Type.MediumArmor, Armor.Type.Shield });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon });                    
                    ExtralSkill(2, new List<string> { "History", "Insight", "Medicine","Persuasion", "Religion"});
                    break;

                //For Druid
                case Class.Druid_CircleOfTheLand:
                case Class.Druid_CircleOfTHeMoon:
                    hitDice.r = 8;
                    primaryAbility.Add("WIS");
                    PlayerWeanponsAdd(new List<string> { "Scimitar" });
                    PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.SimpleRangedWeapon, Weapon.Type.SimpleMeleeWeapon });
                    PlayerArmorsAdd(new List<string> {  "Leather", "Shield" });
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor, Armor.Type.MediumArmor, Armor.Type.Shield });
                    ProficiencyWeaponAdd(new List<string> { "Club", "Dagger", "Dart", "Javelin", "Mace", "Quarterstaff", "Scimitar", "Sickle", "Sling", "Spear" });
                    ExtralSkill(2, new List<string> { "Arcana", "AnimalHandling", "Insight", "Medicine", "Nature", "Perception", "Religion","Survival",});
                    break;

                //For Fighter
                case Class.Fighter_Champion:
                case Class.Fighter_BattleMaster:
                case Class.Fighter_EldritchKnight:
                    hitDice.r = 10;
                    primaryAbility.Add("STR");
                    primaryAbility.Add("DEX");
                    PlayerWeanponsAdd(new List<string> { "Longbow", "Crossbow,light", "Handaxe" });
                    PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.MartialMeleeWeapon, Weapon.Type.MartialRangedWeapon });
                    PlayerArmorsAdd(new List<string> { "Leather", "ChainMail","Sheild"});
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor, Armor.Type.MediumArmor,Armor.Type.HeavyArmor , Armor.Type.Shield });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon, Weapon.Type.MartialMeleeWeapon, Weapon.Type.MartialRangedWeapon });
                    ExtralSkill(2, new List<string> { "Acrobatics", "AnimalHandling", "Athletics", "History", "Insight", "Intimidation", "Persuasion", "Survival" });
                    break;

                //For Monk
                case Class.Monk_WayOfShadow:
                case Class.Monk_WayOfTheOpenHand:
                case Class.Monk_WayOfFourElements:
                    hitDice.r = 8;
                    primaryAbility.Add("DEX");
                    primaryAbility.Add("WIS");
                    PlayerWeanponsAdd(new List<string> { "Shortbow" });
                    PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.SimpleRangedWeapon, Weapon.Type.SimpleMeleeWeapon });
                    //PlayerArmorsAdd(new List<string> { });
                    //ProficiencyArmorAdd(new List<Armor.Type> { });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon });
                    ExtralSkill(2, new List<string> { "Acrobatics", "Athletics", "History", "Religion","Insight", "Stealth" });
                    break;

                //For Palading
                case Class.Paladin_OathOfAncients:
                case Class.Paladin_OathOfDevotion:
                case Class.Paladin_OathOfVengance:
                    hitDice.r = 10;
                    primaryAbility.Add("STR");
                    primaryAbility.Add("CHA");
                    PlayerWeanponsAdd(new List<string> { "Javelin" });
                    PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.MartialMeleeWeapon, Weapon.Type.MartialRangedWeapon, Weapon.Type.SimpleMeleeWeapon });
                    PlayerArmorsAdd(new List<string> { "Shield", "ChainMail" });
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor, Armor.Type.MediumArmor,Armor.Type.HeavyArmor, Armor.Type.Shield });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon,Weapon.Type.MartialRangedWeapon, Weapon.Type.MartialMeleeWeapon });
                    ExtralSkill(2, new List<string> { "Athletics", "Insight", "Medicine", "Intimidation", "Religion", "Persuasion" });
                    break;

                //For Ranger
                case Class.Ranger_Hunter:
                case Class.Ranger_BeastMaster:
                    hitDice.r = 10;
                    primaryAbility.Add("DEX");
                    primaryAbility.Add("WIS");
                    PlayerWeanponsAdd(new List<string> { "Shortsword" , "Longbow" });
                    PlayerWeanponsAdd(new List<Weapon.Type> {Weapon.Type.SimpleMeleeWeapon });
                    PlayerArmorsAdd(new List<string> { "ScaleMail", "Leather"});
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor, Armor.Type.MediumArmor, Armor.Type.Shield });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon,Weapon.Type.MartialMeleeWeapon, Weapon.Type.MartialRangedWeapon });
                    ExtralSkill(3, new List<string> { "Athletics", "Stealth", "Nature","Investigation", "AnimalHandling", "Insight", "Perception", "Survival"});
                    break;

                //For Rogue
                case Class.Rogue_Thief:
                case Class.Rogue_Assassin:
                case Class.Rogue_ArcaneTrickester:
                    hitDice.r = 8;
                    primaryAbility.Add("DEX");
                    PlayerWeanponsAdd(new List<string> { "Rapier", "Shortsword", "Shortbow", "Dagger" });
                    //PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.SimpleRangedWeapon, Weapon.Type.SimpleMeleeWeapon });
                    PlayerArmorsAdd(new List<string> { "Leather"});
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor});
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon });
                    ProficiencyWeaponAdd(new List<string> { "Crossbow,hand", "Longsword", "Shortsword", "Rapier"});
                    ExtralSkill(4, new List<string> { "Acrobatics", "Athletics", "SleightofHand", "Stealth", "Investigation", "Insight", "Perception", "Deception", "Intimidation", "Performance", "Persuasion" });
                    break;

                //For Sorcerer
                case Class.Sorcerer_WildMagic:
                case Class.Sorcerer_DraconicBloodine:
                    hitDice.r = 6;
                    primaryAbility.Add("CHA");
                    PlayerWeanponsAdd(new List<string> { "Crossbow,light", "Dagger" });
                    PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.SimpleRangedWeapon, Weapon.Type.SimpleMeleeWeapon });
                    //PlayerArmorsAdd(new List<string> { "ScaleMail", "Leather", "ChainMail", "Shield" });
                    //ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor, Armor.Type.MediumArmor, Armor.Type.Shield });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon });
                    ProficiencyWeaponAdd(new List<string> { "Dagger", "Dart", "Sling", "Quarterstaff", "Crossbow,light" });
                    ExtralSkill(2, new List<string> { "Arcana", "Deception", "Insight", "Intimidation", "Religion", "Persuasion" });
                    break;

                //For Warlock
                case Class.Warlock_TheArchfey:
                case Class.Warlock_TheFiend:
                case Class.Warlock_ThwGreatOldOne:
                    hitDice.r = 8;
                    primaryAbility.Add("CHA");
                    PlayerWeanponsAdd(new List<string> { "Dagger", "Crossbow,light" });
                    PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.SimpleRangedWeapon, Weapon.Type.SimpleMeleeWeapon });
                    PlayerArmorsAdd(new List<string> { "Leather"});
                    ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor });
                    ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon });
                    ExtralSkill(2, new List<string> { "Arcana", "History", "Nature", "Religion", "Investigation", "Deception", "Intimidation"});
                    break;

                //For Wizard
                case Class.Wizard_SchoolOfIllusion:
                case Class.Wizard_SchoolOfEvocation:
                case Class.Wizard_SchoolOfAbjuration:
                case Class.Wizard_SchoolOfDivination:
                case Class.Wizard_SchoolOfNecromancy:
                case Class.Wizard_SchoolOfConjuration:
                case Class.Wizard_SchoolOfEnchantment:
                case Class.Wizard_SchoolOfTransmutation:
                    hitDice.r = 6;
                    primaryAbility.Add("INT");
                    PlayerWeanponsAdd(new List<string> { "Quarterstaff", "Dagger"});
                    PlayerWeanponsAdd(new List<Weapon.Type> { Weapon.Type.SimpleRangedWeapon, Weapon.Type.SimpleMeleeWeapon });
                    //PlayerArmorsAdd(new List<string> { "ScaleMail", "Leather", "ChainMail", "Shield" });
                    //ProficiencyArmorAdd(new List<Armor.Type> { Armor.Type.LightArmor, Armor.Type.MediumArmor, Armor.Type.Shield });
                    //ProficiencyWeaponAdd(new List<Weapon.Type> { Weapon.Type.SimpleMeleeWeapon, Weapon.Type.SimpleRangedWeapon });
                    ProficiencyWeaponAdd(new List<string> { "Dagger", "Dart", "Sling", "Quarterstaff", "Crossbow,light" });
                    ExtralSkill(2, new List<string> { "Arcana", "History","Religion", "Investigation", "Insight", "Medicine"});
                    break;
            }
        }
        public string GetClass()
        {
            switch (this.pClass)
            {
                case Class.Barbarian_PathOfTheBerserker:
                case Class.Barbarian_PathOfTheWarrior:
                    return "Barbarian";
                case Class.Bard_CollegeOfLore:
                case Class.Bard_CollegeOfValor:
                    return "Bard";
                case Class.Cleric_LifeDomain:
                case Class.Cleric_War_Domain:
                case Class.Cleric_LightDomain:
                case Class.Cleric_NatureDomain:
                case Class.Cleric_TempestDomain:
                case Class.Cleric_TrickeryDomain:
                case Class.Cleric_KnowledgeDomain:
                    return "Cleric";
                case Class.Druid_CircleOfTheLand:
                case Class.Druid_CircleOfTHeMoon:
                    return "Druid";
                case Class.Fighter_Champion:
                case Class.Fighter_BattleMaster:
                case Class.Fighter_EldritchKnight:
                    return "Fighter";
                case Class.Monk_WayOfShadow:
                case Class.Monk_WayOfTheOpenHand:
                case Class.Monk_WayOfFourElements:
                    return "Monk";
                case Class.Paladin_OathOfAncients:
                case Class.Paladin_OathOfDevotion:
                case Class.Paladin_OathOfVengance:
                    return "Paladin";
                case Class.Ranger_Hunter:
                case Class.Ranger_BeastMaster:
                    return "Ranger";
                case Class.Rogue_Thief:
                case Class.Rogue_Assassin:
                case Class.Rogue_ArcaneTrickester:
                    return "Rogue";
                case Class.Sorcerer_WildMagic:
                case Class.Sorcerer_DraconicBloodine:
                    return "Sorcerer";
                case Class.Warlock_TheArchfey:
                case Class.Warlock_TheFiend:
                case Class.Warlock_ThwGreatOldOne:
                    return "Warlock";
                case Class.Wizard_SchoolOfIllusion:
                case Class.Wizard_SchoolOfEvocation:
                case Class.Wizard_SchoolOfAbjuration:
                case Class.Wizard_SchoolOfDivination:
                case Class.Wizard_SchoolOfNecromancy:
                case Class.Wizard_SchoolOfConjuration:
                case Class.Wizard_SchoolOfEnchantment:
                case Class.Wizard_SchoolOfTransmutation:
                    return "Wizard";
            }
            return null;
        }

        public void PlayerWeanponsAdd(List<string> name)
        {
            foreach (string s in name)
            {
                playerWeapons.Add(s);
            }
        }
        public void PlayerWeanponsAdd(List<Weapon.Type> type)
        {
            foreach (Weapon w in eb.allWeapons)
            {
                foreach (Weapon.Type wt in type)
                {
                    if (w.type == wt)
                    {
                        playerWeapons.Add(w.name);
                    }
                }
            }
        }
        public void PlayerArmorsAdd(List<string> name)
                {
                    foreach (string s in name)
                    {
                        playerArmors.Add(s);
                    }
                }
        public void PlayerArmorsAdd(List<Armor.Type> type)
        {
            foreach (Armor a in eb.allArmors)
            {
                foreach (Armor.Type at in type)
                {
                    if (a.type == at)
                    {
                        playerArmors.Add(a.name);
                    }
                }    
            }
        }

        public void ProficiencyArmorAdd(List<Armor.Type> type)
        {
            foreach (Armor a in eb.allArmors)
            {
                foreach (Armor.Type t in type)
                {
                    if (a.type == t)
                        proficiencyArmorList.Add(a.name);
                }
            }
        }
        public void ProficiencyArmorAdd(List<string> armorName)
        {
            foreach (string an in armorName)
            {
                proficiencyArmorList.Add(an);
            }
        }
        public void ProficiencyWeaponAdd(List<Weapon.Type> type)
        {
            foreach (Weapon a in eb.allWeapons)
            {
                foreach (Weapon.Type t in type)
                {
                    if (a.type == t)
                        proficiencyWeaponList.Add(a.name);
                }
            }
        }
        public void ProficiencyWeaponAdd(List<string> weaponName)
        {
            foreach (string wn in weaponName)
            {                
                proficiencyWeaponList.Add(wn);                
            }
        }
        
        public void ExtralAbility(int number, List<string> ability)
        {
            List<string> copy = ability;
            for (int i = 0; i < number; i++)
            {
                int rdm = new Random(Guid.NewGuid().GetHashCode()).Next(0, copy.Count - 1);
                string ab = copy[rdm];
                switch (ab)
                {
                    case "STR":
                        this.ab.STR += 1;
                        break;
                    case "DEX":
                        this.ab.DEX += 1;
                        break;
                    case "CON":
                        this.ab.CON += 1;
                        break;
                    case "INT":
                        this.ab.INT += 1;
                        break;
                    case "CHA":
                        this.ab.CHA += 1;
                        break;
                    case "WIS":
                        this.ab.WIS += 1;
                        break;
                }
                copy.RemoveAt(rdm);
            }
        }
        public void ExtralSkill(int number, List<string> skills)
        {
            List<string> copy = skills;
            for (int i = 0; i < number; i++)
            {
                int rdm = new Random(Guid.NewGuid().GetHashCode()).Next(0, copy.Count - 1);
                string randomSkill = copy[rdm];
                copy.RemoveAt(rdm);
                AddSkill(randomSkill, 1);
            }
        }

        // Add new weapon to player weapon list
        public void AddWeapon(EquipmentBase eb, string weapon)
        {
            foreach (Weapon wp in eb.allWeapons)
            {
                if (wp.name == weapon)
                    weapons.Add(wp);
            }
        }
        public void AddArmor(EquipmentBase eb, string armorName)
        {
            foreach (Armor am in eb.allArmors)
            {
                if (am.name == armorName)
                    armors.Add(am);
            }
        }

        //set ability base on the primary ability
        public List<int> AbilityScore = new List<int>() { 15, 14, 13, 12, 10, 8 };
        public void AutoSetAbility(List<string> primaryAbility)
        {
            List<int> AbilityScoreCopy = AbilityScore;

            string p1 = primaryAbility[0];
            string p2 = (primaryAbility.Count > 2) ? primaryAbility[1] : "null";

            switch (p1)
            {
                case "STR":
                    ab.STR = AbilityScoreCopy[0];
                    AbilityScoreCopy.RemoveAt(0);
                    break;
                case "DEX":
                    ab.DEX = AbilityScoreCopy[0];
                    AbilityScoreCopy.RemoveAt(0);
                    break;
                case "CON":
                    ab.CON = AbilityScoreCopy[0];
                    AbilityScoreCopy.RemoveAt(0);
                    break;
                case "WIS":
                    ab.WIS = AbilityScoreCopy[0];
                    AbilityScoreCopy.RemoveAt(0);
                    break;
                case "CHA":
                    ab.CHA = AbilityScoreCopy[0];
                    AbilityScoreCopy.RemoveAt(0);
                    break;
                case "INT":
                    ab.INT = AbilityScoreCopy[0];
                    AbilityScoreCopy.RemoveAt(0);
                    break;
            }
            if (p2 != "null")
            {
                switch (p2)
                {
                    case "STR":
                        ab.STR = AbilityScoreCopy[0];
                        AbilityScoreCopy.RemoveAt(0);
                        break;
                    case "DEX":
                        ab.DEX = AbilityScoreCopy[0];
                        AbilityScoreCopy.RemoveAt(0);
                        break;
                    case "CON":
                        ab.CON = AbilityScoreCopy[0];
                        AbilityScoreCopy.RemoveAt(0);
                        break;
                    case "WIS":
                        ab.WIS = AbilityScoreCopy[0];
                        AbilityScoreCopy.RemoveAt(0);
                        break;
                    case "CHA":
                        ab.CHA = AbilityScoreCopy[0];
                        AbilityScoreCopy.RemoveAt(0);
                        break;
                    case "INT":
                        ab.INT = AbilityScoreCopy[0];
                        AbilityScoreCopy.RemoveAt(0);
                        break;
                }
            }

            if (ab.STR == 0)
            {
                int r = new Random(Guid.NewGuid().GetHashCode()).Next(0, AbilityScoreCopy.Count - 1);
                ab.STR = AbilityScoreCopy[r];
                AbilityScoreCopy.RemoveAt(r);
            }
            if (ab.DEX == 0)
            {
                int r = new Random(Guid.NewGuid().GetHashCode()).Next(0, AbilityScoreCopy.Count - 1);
                ab.DEX = AbilityScoreCopy[r];
                AbilityScoreCopy.RemoveAt(r);
            }
            if (ab.CHA == 0)
            {
                int r = new Random(Guid.NewGuid().GetHashCode()).Next(0, AbilityScoreCopy.Count - 1);
                ab.CHA = AbilityScoreCopy[r];
                AbilityScoreCopy.RemoveAt(r);
            }
            if (ab.WIS == 0)
            {
                int r = new Random(Guid.NewGuid().GetHashCode()).Next(0, AbilityScoreCopy.Count - 1);
                ab.WIS = AbilityScoreCopy[r];
                AbilityScoreCopy.RemoveAt(r);
            }
            if (ab.CON == 0)
            {
                int r = new Random(Guid.NewGuid().GetHashCode()).Next(0, AbilityScoreCopy.Count - 1);
                ab.CON = AbilityScoreCopy[r];
                AbilityScoreCopy.RemoveAt(r);
            }
            if (ab.INT == 0)
            {
                ab.INT = AbilityScoreCopy[0];
            }

        }

        public Player GetCopy()
        {
            return new Player(pRace, pClass, eb, proficiencyBouns);
        }
        public Weapon GetWeapon()
        {
            if (weapon != null)
                return weapon;
            else
                return new Weapon("nothing", Weapon.Type.Null, new Dice("0D0-10"),Weapon.Range.Null, false);
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
        public void AddSkill(string skill, int proficiency)
        {
            switch (skill)
            {
                case "Acrobatics":
                    skills.Acrobatics += proficiency;
                    break;
                case "Athletics":
                    skills.Athletics += proficiency;
                    break;
                case "SleightofHand":
                    skills.SleightofHand += proficiency;
                    break;
                case "Stealth":
                    skills.Stealth += proficiency;
                    break;
                case "Arcana":
                    skills.Arcana += proficiency;
                    break;
                case "History":
                    skills.History += proficiency;
                    break;
                case "Nature":
                    skills.Nature += proficiency;
                    break;
                case "Religon":
                    skills.Religion += proficiency;
                    break;
                case "Investigation":
                    skills.Investigation += proficiency;
                    break;
                case "AnimalHandling":
                    skills.AnimalHandling += proficiency;
                    break;
                case "Insight":
                    skills.Insight += proficiency;
                    break;
                case "Medicine":
                    skills.Medicine += proficiency;
                    break;
                case "Perception":
                    skills.Perception += proficiency;
                    break;
                case "Survival":
                    skills.Survival += proficiency;
                    break;
                case "Deception":
                    skills.Deception += proficiency;
                    break;
                case "Intimidation":
                    skills.Intimidation += proficiency;
                    break;
                case "Performance":
                    skills.Performance += proficiency;
                    break;
                case "Persuasion":
                    skills.Persuasion += proficiency;
                    break;
            }
        }
        public void ClearProficiencyWeapon()
        {
            this.proficiencyWeaponList.Clear();
        }

        

        //public Weapon FindWeapon(string name)
        //{
        //    foreach (Weapon wp in weapons)
        //    {
        //        if (wp.name == name)
        //            return wp;
        //    }
        //    return new Weapon("", new Dice("0D0+0"), Weapon.Range.Melee, false);
        //}
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
    class Skills : Status
    {
        List<int> SkillsList = new List<int> {};

        public int Acrobatics, Athletics,SleightofHand, Stealth, Arcana, History, Nature, Religion, Investigation, AnimalHandling, Insight, Medicine, Perception, Survival, Deception, Intimidation, Performance, Persuasion;
               

        public Skills(AbilityMod abm)
        {
            //Str
            this.Athletics       += abm.STR;

            //Dex
            this.SleightofHand   += abm.DEX;
            this.Stealth         += abm.DEX;
            this.Acrobatics      += abm.DEX;

            //Con

            //Int
            this.Arcana          += abm.INT;
            this.History         += abm.INT;
            this.Nature          += abm.INT;
            this.Religion        += abm.INT;
            this.Investigation   += abm.INT;

            //Wis
            this.AnimalHandling  += abm.WIS;
            this.Insight         += abm.WIS;
            this.Medicine        += abm.WIS;
            this.Perception      += abm.WIS;
            this.Survival        += abm.WIS;

            //Cha
            this.Deception       += abm.CHA;
            this.Intimidation    += abm.CHA;
            this.Performance     += abm.CHA;
            this.Persuasion      += abm.CHA;
        }
        public static double SkillCheck(int skill, int times, Player p)
        {
            int veryEasySuccess = 0;            //5
            int easySuccess = 0;                //10
            int mediumSuccess = 0;              //15
            int hardSuccess = 0;                //20
            int veryHardSuccess = 0;            //25
            int nearlyImpossibleSuccess = 0;    //30

            for (int i = 0; i < times; i++)
            {
                SkillsSuccessCheck(p.nad, skill);

                bool ves = (SkillsSuccessCheck(p.nad, skill) > 5);
                if (p.ExtrallCapacity.Contains("RerollD20") && ves == false) // reroll if 1) has RerollD20 Capacity 2) fail
                    ves = (SkillsSuccessCheck(p.nad, skill) > 5);
                veryEasySuccess = ves  ? veryEasySuccess + 1 : veryEasySuccess;

                bool es = (SkillsSuccessCheck(p.nad, skill) > 10);
                if (p.ExtrallCapacity.Contains("RerollD20") && es == false) // reroll if 1) has RerollD20 Capacity 2) fail
                    es = (SkillsSuccessCheck(p.nad, skill) > 10);
                easySuccess = es ? easySuccess + 1 : easySuccess;

                bool ms = (SkillsSuccessCheck(p.nad, skill) > 15);
                if (p.ExtrallCapacity.Contains("RerollD20") && ms == false) // reroll if 1) has RerollD20 Capacity 2) fail
                    ms = (SkillsSuccessCheck(p.nad, skill) > 15);
                mediumSuccess = ms ? mediumSuccess + 1 : mediumSuccess;

                bool hs = (SkillsSuccessCheck(p.nad, skill) > 20);
                if (p.ExtrallCapacity.Contains("RerollD20") && hs == false) // reroll if 1) has RerollD20 Capacity 2) fail
                    hs = (SkillsSuccessCheck(p.nad, skill) > 20);
                hardSuccess = hs ? hardSuccess + 1 : hardSuccess;

                bool vhs = (SkillsSuccessCheck(p.nad, skill) > 25);
                if (p.ExtrallCapacity.Contains("RerollD20") && vhs == false) // reroll if 1) has RerollD20 Capacity 2) fail
                    vhs = (SkillsSuccessCheck(p.nad, skill) > 25);
                veryHardSuccess = vhs ? veryHardSuccess + 1 : veryHardSuccess;

                bool nis = (SkillsSuccessCheck(p.nad, skill) > 30);
                if (p.ExtrallCapacity.Contains("RerollD20") && nis == false) // reroll if 1) has RerollD20 Capacity 2) fail
                    nis = (SkillsSuccessCheck(p.nad, skill) > 30);
                nearlyImpossibleSuccess = nis ? nearlyImpossibleSuccess + 1 : nearlyImpossibleSuccess;
            }

            double result = (Convert.ToDouble(veryEasySuccess) / Convert.ToDouble(times)) * 12 +
                            (Convert.ToDouble(easySuccess) / Convert.ToDouble(times)) * 18 +
                            (Convert.ToDouble(mediumSuccess) / Convert.ToDouble(times)) * 30 +
                            (Convert.ToDouble(hardSuccess) / Convert.ToDouble(times)) * 18 +
                            (Convert.ToDouble(veryHardSuccess) / Convert.ToDouble(times)) * 12 +
                            (Convert.ToDouble(nearlyImpossibleSuccess) / Convert.ToDouble(times)) * 10;

            return result;
        }

        public static int SkillsSuccessCheck(NAD nadstate, int skill)
        {
            int diceResult1 = Dice.RollDice("1D20+0");
            int diceResult2 = Dice.RollDice("1D20+0");
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
            return diceResult + skill;
        }
        public double SkillsCheck(double[] checkList, int times,Player p)
        {
            AddSkills();

            double totalScore = 0;

            for (int s = 0; s < SkillsList.Count; s++)
            {
                if (checkList[s] != 0)
                {
                    double result = 0;
                    p.nad = NAD.Advantage;
                    result += SkillCheck(SkillsList[s], times * 3, p);
                    p.nad = NAD.Normal;
                    result += SkillCheck(SkillsList[s], times * 8, p);
                    p.nad = NAD.Disadvantage;
                    result += SkillCheck(SkillsList[s], times * 3,p);

                    totalScore += result * checkList[s];
                }
            }
            return totalScore;
        }
        
        public void AddSkills()
        {
            SkillsList.Clear();
            SkillsList.Add(Acrobatics);
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

    }
    //enemy
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
    class Enemy : Unit
    {
        public string name;
        //public int armorClass;

        public Dice hitDice;
        //public int maxHealth;
        //public int health;

        //public Ability ab;
        //public AbilityMod abm;

        public EnemyAttack ea;

        public Enemy(string name,int armorClass, Dice hitDice, Ability ab , EnemyAttack ea)
        {
            this.name = name;
            this.armorClass = armorClass;
            this.hitDice = hitDice;

            this.maxHealth = Dice.RollDice(hitDice.n + "D" + hitDice.r + "+" + hitDice.i);
            this.health = maxHealth;

            this.ab = ab;
            this.abm = new AbilityMod(ab);

            this.ea = ea;
        }

        public Enemy GetCopy()
        {
            Enemy enemyCopy = new Enemy(name, armorClass, hitDice, ab, ea);
            //enemyCopy.health = this.health;
           // enemyCopy.maxHealth = this.maxHealth;
            return enemyCopy;
        }
        public void Ini()
        {
            ab = new Ability(0, 0, 0, 0, 0, 0);
        }
    }

    //combat
    class Weapon
    {
        public enum Range
        {
            Melee,
            Ranged,
            Null
        }
        public enum Type
        {
            SimpleMeleeWeapon,
            SimpleRangedWeapon,
            MartialMeleeWeapon,
            MartialRangedWeapon,
            Null
        }

        public string name;
        public Type type;
        public Dice damageDice;
        public Range range;
        public bool isFiness;

        //public string[] meleeWeaponList = { "Club", "Dagger", "Greatclub", "Handaxe", "Javelin", "Light hammer", "Light hammer", "Mace", "Quarterstaff", "Sickle", "Spear" };
        //public string[] rangedWeaponList = { "Crossbow,light", "Dart", "Shortbow", "Sling" };
        //public string[] martialMeleeWeaponList = { "Battleaxe", "Flail", "Glavie", "GreatAxe", "Greatsword", "Halberd", "Lance", "Longsword", "Maul", "Morningstar", "Pike", "Rapier", "Scimitar", "Shortword", "Trident", "War pick", "Warhammer", "Whip" };
        //public string[] martialRangedWeaponList = { "Blowgun", "Crossbow,hand", "Crossbow,heavy", "Longbow", "Net" };

        public Weapon(string name, Type type, Dice damageDice, Range range, bool isFiness)//,int bouns)
        {
            this.name = name;
            this.type = type;
            this.damageDice = damageDice;
            this.range = range;
            this.isFiness = isFiness;
        }
    }
    class Armor : Status
    {
        //public string[] lightArmorList = { "Padded", "Leather", "StuddedLeather" };
        //public string[] mediumArmorList = { "Hide", "ChainShirt", "ScaleMail", "BreastPlate", "HalfPlate" };
        //public string[] HeavyArmor = { "RingMail", "ChainMail", "Splint", "Plate" };
        //public string[] Shield = { "Shield" };

        public string name;
        public enum Type
        {
            LightArmor,
            MediumArmor,
            HeavyArmor,
            Shield
        }
        public Type type;
        public int armorClass;
        public ACMod acm;
        public int StrNeed;
        public NAD Stealth;

        public Armor(string name, Type type, int armorClass, ACMod acm, int StrNeed, NAD Stealth)
        {
            this.name = name;
            this.type = type;
            this.armorClass = armorClass;
            this.acm = acm;
            this.StrNeed = StrNeed;
            this.Stealth = Stealth;
        }

        public int GetAC(Player p)
        {
            return (armorClass + ((acm.isDexMod) ? ((p.abm.DEX > acm.MaxMod) ? acm.MaxMod : p.abm.DEX) : 0));
        }
    }
    class ACMod
    {
        public bool isDexMod;
        public int MaxMod;

        public ACMod(bool isDexMod, int MaxMod)
        {
            this.isDexMod = isDexMod;
            this.MaxMod = MaxMod;
        }
    }
    class AttackChecker
    {
        public bool isAttackSuccess;
        public bool isCritical;

        public AttackChecker(bool isAttackSuccess, bool isCritical)
        {
            this.isAttackSuccess = isAttackSuccess;
            this.isCritical = isCritical;
        }
    }

    //database
    class UnitBase
    {
        public EquipmentBase eb;

        public List<Enemy> enemies = new List<Enemy>();
        public List<Player> players = new List<Player>();

        public UnitBase(EquipmentBase eb)
        {
            this.eb = eb;
            Ini();
        }

        public void Ini()
        {
            AddPlayers();
            AddEnemies();
        }
        //Enemy        
        public void AddEnemies()
        {
            //                    |        Name      | AC |     Hit point      |            Ability                |   EnemyAttack >>[     Melee Attack   ][   Ranged Attack  ][MOD]|      
            enemies.Add(new Enemy("Bat"              , 12 , new Dice("1D4-1")   , new Ability(2, 15, 8, 2, 12, 4)   , new EnemyAttack(new Dice("null")  , 0 ,new Dice("null")   , 0, 1)));
            enemies.Add(new Enemy("Black Bear"       , 11 , new Dice("3D8+6")   , new Ability(15, 10, 14, 2, 12, 7) , new EnemyAttack(new Dice("1D6+2") , 3 ,new Dice("2D4+2")  , 3, 0)));
            enemies.Add(new Enemy("Boar"             , 11 , new Dice("2D8+2")   , new Ability(13, 11, 12, 2, 9, 5)  , new EnemyAttack(new Dice("1D6+1") , 3 ,new Dice("null")   , 3, 0)));
            enemies.Add(new Enemy("Brown Bear"       , 11 , new Dice("4D10+12") , new Ability(19, 10, 16, 2, 13, 7) , new EnemyAttack(new Dice("1D8+4") , 5 ,new Dice("2D6+4")  , 5, 0)));
            enemies.Add(new Enemy("Cat"              , 12 , new Dice("1D4+0")   , new Ability(3, 15, 10, 3, 12, 7)  , new EnemyAttack(new Dice("null")  , 0 ,new Dice("null")   , 0, 1)));
            enemies.Add(new Enemy("Constrictor Snake", 12 , new Dice("2D10+2")  , new Ability(15, 14, 12, 1, 10, 3) , new EnemyAttack(new Dice("1D6+2") , 4 ,new Dice("1D8+2")  , 4, 0)));
            enemies.Add(new Enemy("Crocodile"        , 12 , new Dice("3D10+3")  , new Ability(15, 10, 13, 2, 10, 5) , new EnemyAttack(new Dice("1D10+2"), 4 ,new Dice("null")   , 0, 0)));
            enemies.Add(new Enemy("Dire Wolf"        , 14 , new Dice("5D10+10") , new Ability(17, 15, 15, 3, 12, 7) , new EnemyAttack(new Dice("2D6+3") , 5 ,new Dice("null")   , 0, 0)));
            enemies.Add(new Enemy("Frog"             , 11 , new Dice("1D4-1")   , new Ability(1, 13, 8, 1, 8, 3)    , new EnemyAttack(new Dice("null")  , 0 ,new Dice("null")   , 0, 0)));
            enemies.Add(new Enemy("Giant Eagle"      , 13 , new Dice("4D10+4")  , new Ability(16, 17, 13, 8, 14, 10), new EnemyAttack(new Dice("1D6+3") , 5 ,new Dice("2D6+3")  , 5, 0)));
            enemies.Add(new Enemy("Giant Spider"     , 14 , new Dice("4D10+4")  , new Ability(14, 16, 12, 2, 11, 4) , new EnemyAttack(new Dice("1D8+3") , 5 ,new Dice("null")   , 0, 0)));
            enemies.Add(new Enemy("Hawk(Falcon)"     , 13 , new Dice("1D4-1")   , new Ability(5,16,8,2,14,6)        , new EnemyAttack(new Dice("0D0+1") , 5 ,new Dice("null")   , 0, 0)));
            enemies.Add(new Enemy("Imp"              , 13 , new Dice("3D4+3")   , new Ability(6, 17, 13, 11, 12, 14), new EnemyAttack(new Dice("1D4+3") , 5 ,new Dice("null")   , 0, 0)));
            enemies.Add(new Enemy("Lion"             , 12 , new Dice("4D10 + 4"), new Ability(17, 15, 13, 3, 12, 8) , new EnemyAttack(new Dice("1D8+3") , 5 ,new Dice("1D6+3")  , 5, 0)));
            enemies.Add(new Enemy("Mastiff"          , 12 , new Dice("1D8+1")   , new Ability(13, 14, 12, 3, 12, 7) , new EnemyAttack(new Dice("1D6+1") , 3 ,new Dice("null")   , 0, 0)));
            enemies.Add(new Enemy("Mule"             , 10 , new Dice("2D8+2")   , new Ability(14, 10, 13, 2, 10, 5) , new EnemyAttack(new Dice("1D4+2") , 2 ,new Dice("null")   , 0, 0)));


        }
        //Player        
        public void AddPlayers()
        {
            //                    |         Race              |               Class                      |pb|  
            //players.Add(new Player(Player.Race.Halfling_Lightfoot, Player.Class.Barbarian_PathOfTheBerserker, eb, 2 ));
            //players.Add(new Player(Player.Race.Halfling_Stout    , Player.Class.Barbarian_PathOfTheBerserker, eb, 2));       

            for (int r = 1; r < 15; r++)
            {
                for (int c = 1; c < 13; c++)
                {
                    players.Add(new Player(ChoseRace(r), ChoseClass(c), eb, 2));
                }
            }
        }
        public static Player.Race ChoseRace(int Range_1_14)
        {
            switch (Range_1_14)
            {
                case 1: return Player.Race.Dwarf_HillDwarf;
                case 2: return Player.Race.Dwarf_MountainDwarf;
                case 3: return Player.Race.Elf_HighElf;
                case 4: return Player.Race.Elf_WoddElf;
                case 5: return Player.Race.Elf_DarkElf;
                case 6: return Player.Race.Halfling_Lightfoot;
                case 7: return Player.Race.Halfling_Stout;
                case 8: return Player.Race.Human;
                case 9: return Player.Race.Dragonborn;
                case 10: return Player.Race.Gnome_ForestGnome;
                case 11: return Player.Race.Gnome_RockGnome;
                case 12: return Player.Race.HalfElf;
                case 13: return Player.Race.HalfOrc;
                case 14: return Player.Race.Tiefling;
            }
            return Player.Race.Human;
        }
        public static Player.Class ChoseClass(int Range_1_12)
        {
            switch (Range_1_12)
            {
                case 1: return Player.Class.Barbarian_PathOfTheBerserker;
                case 2: return Player.Class.Bard_CollegeOfLore;
                case 3: return Player.Class.Cleric_KnowledgeDomain;
                case 4: return Player.Class.Druid_CircleOfTheLand;
                case 5: return Player.Class.Fighter_BattleMaster;
                case 6: return Player.Class.Monk_WayOfFourElements;
                case 7: return Player.Class.Paladin_OathOfAncients;
                case 8: return Player.Class.Ranger_BeastMaster;
                case 9: return Player.Class.Rogue_ArcaneTrickester;
                case 10: return Player.Class.Sorcerer_DraconicBloodine;
                case 11: return Player.Class.Warlock_TheArchfey;
                case 12: return Player.Class.Wizard_SchoolOfAbjuration;
            }
            return Player.Class.Fighter_BattleMaster;
        }

    }
    class EquipmentBase
    {      
        public List<Weapon> allWeapons = new List<Weapon>();
        public List<Armor> allArmors = new List<Armor>();

        public EquipmentBase()
        {
            InitialDataBase();
        }

        public void InitialDataBase()
        {
            AddWeapons();            
            AddArmors();
        }     
        //Weapon        
        public void AddWeapons()
        {
            //name   diceNumber    diceType    isMelee    isFiness

            //simple melee weapons
            allWeapons.Add(new Weapon("Club",Weapon.Type.SimpleMeleeWeapon, new Dice("1D4 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Dagger", Weapon.Type.SimpleMeleeWeapon, new Dice("1D4 + 0"), Weapon.Range.Melee, true));
            allWeapons.Add(new Weapon("Greatclub", Weapon.Type.SimpleMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Handaxe", Weapon.Type.SimpleMeleeWeapon, new Dice("1D6 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Javelin", Weapon.Type.SimpleMeleeWeapon, new Dice("1D6 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Light hammer", Weapon.Type.SimpleMeleeWeapon, new Dice("1D4 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Mace", Weapon.Type.SimpleMeleeWeapon, new Dice("1D6 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Quarterstaff", Weapon.Type.SimpleMeleeWeapon, new Dice("1D6 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Sickle", Weapon.Type.SimpleMeleeWeapon, new Dice("1D4 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Spear", Weapon.Type.SimpleMeleeWeapon, new Dice("1D6 + 0"), Weapon.Range.Melee, false));

            //simple ranged weapons 
            allWeapons.Add(new Weapon("Crossbow,light", Weapon.Type.SimpleRangedWeapon, new Dice("1D8 + 0"), Weapon.Range.Ranged, false));
            allWeapons.Add(new Weapon("Dart", Weapon.Type.SimpleRangedWeapon, new Dice("1D4 + 0"), Weapon.Range.Ranged, true));
            allWeapons.Add(new Weapon("Shortbow", Weapon.Type.SimpleRangedWeapon, new Dice("1D6 + 0"), Weapon.Range.Ranged, false));
            allWeapons.Add(new Weapon("Sling", Weapon.Type.SimpleRangedWeapon, new Dice("1D4 + 0"), Weapon.Range.Ranged, false));

            //Martial melee wapons
            allWeapons.Add(new Weapon("Battleaxe", Weapon.Type.MartialMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Flail", Weapon.Type.MartialMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Glavie", Weapon.Type.MartialMeleeWeapon, new Dice("1D10 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("GreatAxe", Weapon.Type.MartialMeleeWeapon, new Dice("1D12 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Greatsword", Weapon.Type.MartialMeleeWeapon, new Dice("2D6 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Halberd", Weapon.Type.MartialMeleeWeapon, new Dice("1D10 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Lance", Weapon.Type.MartialMeleeWeapon, new Dice("1D12 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Longsword", Weapon.Type.MartialMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Maul", Weapon.Type.MartialMeleeWeapon, new Dice("2D6 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Morningstar", Weapon.Type.MartialMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Pike", Weapon.Type.MartialMeleeWeapon, new Dice("1D10 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Rapier", Weapon.Type.MartialMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, true));
            allWeapons.Add(new Weapon("Scimitar", Weapon.Type.MartialMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, true));
            allWeapons.Add(new Weapon("Shortsword", Weapon.Type.MartialMeleeWeapon, new Dice("1D6 + 0"), Weapon.Range.Melee, true));
            allWeapons.Add(new Weapon("Trident", Weapon.Type.MartialMeleeWeapon, new Dice("1D6 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("War pick", Weapon.Type.MartialMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Warhammer", Weapon.Type.MartialMeleeWeapon, new Dice("1D8 + 0"), Weapon.Range.Melee, false));
            allWeapons.Add(new Weapon("Whip", Weapon.Type.MartialMeleeWeapon, new Dice("1D4 + 0"), Weapon.Range.Melee, true));

            //Martial Ranged 
            allWeapons.Add(new Weapon("Blowgun", Weapon.Type.MartialRangedWeapon, new Dice("0D0 + 1"), Weapon.Range.Ranged, false));
            allWeapons.Add(new Weapon("Crossbow,hand", Weapon.Type.MartialRangedWeapon, new Dice("1D6 + 0"), Weapon.Range.Ranged, false));
            allWeapons.Add(new Weapon("Crossbow,heavy", Weapon.Type.MartialRangedWeapon, new Dice("1D10 + 0"), Weapon.Range.Ranged, false));
            allWeapons.Add(new Weapon("Longbow", Weapon.Type.MartialRangedWeapon, new Dice("1D8 + 0"), Weapon.Range.Ranged, false));
            allWeapons.Add(new Weapon("Net", Weapon.Type.MartialRangedWeapon, new Dice("0D0 + 0"), Weapon.Range.Ranged, false));
        }
        //Armor        
        public void AddArmors()
        {
            //light Armor
            //                      |        name      |         type              |AC| is add DexMod |MAX     |SN|     stealth            |
            allArmors.Add(new Armor("Padded"           ,Armor.Type.LightArmor     ,11 ,new ACMod(true ,99)    ,0 ,Status.NAD.Disadvantage));
            allArmors.Add(new Armor("Leather"          ,Armor.Type.LightArmor     ,11 ,new ACMod(true ,99)    ,0 ,Status.NAD.Normal));
            allArmors.Add(new Armor("StuddedLeather"   ,Armor.Type.LightArmor     ,12 ,new ACMod(true ,99)    ,0 ,Status.NAD.Normal));

            //Medium Armor
            allArmors.Add(new Armor("Hide"             ,Armor.Type.MediumArmor    ,12 ,new ACMod(true ,2)     ,0 ,Status.NAD.Normal));
            allArmors.Add(new Armor("ChainShirt"       ,Armor.Type.MediumArmor    ,13 ,new ACMod(true ,2)     ,0 ,Status.NAD.Normal));
            allArmors.Add(new Armor("ScaleMail"        ,Armor.Type.MediumArmor    ,14 ,new ACMod(true ,2)     ,0 ,Status.NAD.Disadvantage));
            allArmors.Add(new Armor("Breastplate"      ,Armor.Type.MediumArmor    ,14 ,new ACMod(true ,2)     ,0 ,Status.NAD.Normal));
            allArmors.Add(new Armor("HalfPlate"        ,Armor.Type.MediumArmor    ,15 ,new ACMod(true ,2)     ,0 ,Status.NAD.Disadvantage));

            //Heavy Armor
            allArmors.Add(new Armor("RingMail"         ,Armor.Type.HeavyArmor     ,14 ,new ACMod(false,0)     ,0 ,Status.NAD.Disadvantage));
            allArmors.Add(new Armor("ChainMail"        ,Armor.Type.HeavyArmor     ,16 ,new ACMod(false,0)     ,13,Status.NAD.Disadvantage));
            allArmors.Add(new Armor("Splint"           ,Armor.Type.HeavyArmor     ,17 ,new ACMod(false,0)     ,15,Status.NAD.Disadvantage));
            allArmors.Add(new Armor("Plate"            ,Armor.Type.HeavyArmor     ,18 ,new ACMod(false,0)     ,15,Status.NAD.Disadvantage));

            //Shield
            allArmors.Add(new Armor("Shield"           ,Armor.Type.Shield         , 2 ,new ACMod(false,0)     ,0 ,Status.NAD.Normal));
        }
    }
}
