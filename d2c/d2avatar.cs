using SCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfAnimatedGif;

namespace d2c
{
    public class d2avatar
    {
        public List<Image> parts;
        public List<string> partcodes;
        public List<string> partpaths;
        public List<string> orPaths;
        public List<string> partstrings {get; set;}
        Image testImage = new Image();
        public string d2class = "am";
        public string Name { get; set; }
        string mode = "tn";
        public int classId = 0;
        public int armorId = 0;
        public int[, ,] partproperties;
        public int[] overrideLeft;
        public int[] overrideTop;
        public int[] posLeft { get; set; }
        public int[] posTop { get; set; }
        public int[] posZ { get; set; }
        public int[] overridePos { get; set; }
        public bool overRide = false;
        public string overrideString { get; set; }
        public SolidColorBrush nameColour = (SolidColorBrush)new BrushConverter().ConvertFromString("#C4C4C4");
        string stance = "hth";
        string[] backupstance;
        public Visibility squelched {get; set;}
        public Visibility d2CharVis {get; set;}
        public Visibility overrideVis{get; set;}
        BooleanToVisibilityConverter bvc = new BooleanToVisibilityConverter();
       
        /// <summary>
        /// No-arg constructor.
        /// </summary>
        public d2avatar()
        {
            setup();

        }

        /// <summary>
        /// The essentials.
        /// </summary>
        public void setup()
        {/*
             * PartProps array: [class, part, property]
             * Properties: 0=Canvas.Left, 1=Canvas.Top, 2=Panel.ZIndex
             */
            partproperties = new int[5, 9, 3] {
            /*AMAZON
             */
            {
            //Head
            { 17, 14, 8 },
            //Left Arm
            { 12, 26, 3 },
            //Legs
            {4, 49, 1 },
            //Right Arm
            {-5, 27, 5 },
            //Right Hand
            {-4, 52, 2 },
            //Right Shoulder (S1)
            {12, 25, 7 },
            //Left Shoulder (S2)
            {25, 24, 6 },
            //Shield
            {24, 43, 0 },
            //Torso
            {9, 26, 4 } 
            }, 
            /*NECROMANCER
             */
            {
            //Head
            { 23, 13, 8 },
            //Left Arm
            { 14, 27, 3 },
            //Legs
            {4, 49, 1 },
            //Right Arm
            {-3, 24, 5 },
            //Right Hand
            {-4, 49, 0 },
            //Right Shoulder (S1)
            {12, 25, 7 },
            //Left Shoulder (S2)
            {26, 24, 6 },
            //Shield
            {27, 39, 0 },
            //Torso
            {9, 26, 4 } 
            }, 
            /*BARBARIAN
             */
            {
            //Head
            { 25, 13, 8 },
            //Left Arm
            { 19, 27, 4 },
            //Legs
            {4, 49, 1 },
            //Right Arm
            {-10, 29, 5 },
            //Right Hand
            {-4, 49, 0 },
            //Right Shoulder (S1)
            {9, 23, 7 },
            //Left Shoulder (S2)
            {30, 22, 6 },
            //Shield
            {27, 39, 0 },
            //Torso
            {9, 26, 3 } 
            }, 
            /*SORCERESS
             */
            {
            //Head
            { 12, 27, 7 },
            //Left Arm
            { 15, 31, 3 },
            //Legs
            {4, 49, 1 },
            //Right Arm
            {-2, 30, 5 },
            //Right Hand
            {-4, 49, 0 },
            //Right Shoulder (S1)
            {14, 26, 8 },
            //Left Shoulder (S2)
            {27, 29, 7 },
            //Shield
            {27, 39, 0 },
            //Torso
            {11, 26, 6 } 
            }, 
            /*PALADIN
             */
            {
            //Head
            { 24, 13, 8 },
            //Left Arm
            { 15, 28, 3 },
            //Legs
            {5, 49, 1 },
            //Right Arm
            {-6, 28, 4 },
            //Right Hand
            {-4, 49, 0 },
            //Right Shoulder (S1)
            {11, 25, 7 },
            //Left Shoulder (S2)
            {27, 26, 6 },
            //Shield
            {27, 39, 0 },
            //Torso
            {9, 27, 5 } 
            },
                };
            //If the code can't find a particular gif of the intended stance, go to this one.
            backupstance = new string[5] {
             "1ht", //Amazon 
             "1ht", //Necromancer
             "hth", //Barbarian
             "hth", //Sorceress
             "1hs", //Paladin
            };
            overrideLeft = new int[10] {
                -4,-4,-4,-4,0,-4,-12,-4,-4,-4
            };
            overrideTop = new int[10] {
                18,18,26,22,20,26,20,26,18,26
            };
            overridePos = new int[2];
            squelched = new Visibility();
            squelched = Visibility.Hidden;
            d2CharVis = new Visibility();
            d2CharVis = Visibility.Visible;
            overrideVis = new Visibility();
            overrideVis = Visibility.Hidden;
            parts = new List<Image>();
            partstrings = new List<string>();
            orPaths = new List<string>();
            posLeft = new int[9];
            posTop = new int[9];
            posZ = new int[9];
            for (int i = 0; i < 9; i++)
            {
                partstrings.Add("");
            }
            overrideString = "";
            partcodes = new List<string>();
            partpaths = new List<string>();
            partcodes.Add("hd");
            partcodes.Add("la");
            partcodes.Add("lg");
            partcodes.Add("ra");
            partcodes.Add("rh");
            partcodes.Add("s1");
            partcodes.Add("s2");
            partcodes.Add("sh");
            partcodes.Add("tr");
            partpaths.Add("lit");  //HEAD
            partpaths.Add("lit");  //Left Arm
            partpaths.Add("lit");  //Legs
            partpaths.Add("lit");  //Right Arm
            partpaths.Add("jav");  //Right Hand
            partpaths.Add("lit");  //Right Shoulder
            partpaths.Add("lit");  //Left Shoulder
            partpaths.Add("buc");  //Shield
            partpaths.Add("lit");  //Torso
            orPaths.Add("open");
            orPaths.Add("dead");
            orPaths.Add("chat");
            orPaths.Add("drtl");
            orPaths.Add("star");
            orPaths.Add("sexp");
            orPaths.Add("w2bn");
            orPaths.Add("war3");
            orPaths.Add("OPER");
            orPaths.Add("spkr");

        }

        /// <summary>
        /// Sets up the strings for the various gifs that combine to form the avatar.
        /// </summary>
        public void refreshAnimation()
        {
            if (classId < 5)
            {
                try
                {
                    for (int i = 0; i < partcodes.Count; i++)
                    {
                        try
                        {
                            ImageBehavior.SetAnimatedSource(testImage, ResourceHelper.LoadBitmapFromResource("char/"
                                + d2class + "/" + partcodes[i] + "/" + d2class + partcodes[i] + partpaths[i] + mode + stance + ".gif"));
                            //We do this testImage thing first to make sure the image can be loaded.
                            partstrings[i] = "/d2c;component\\char\\" + d2class + "\\" + partcodes[i] + "\\" + d2class
                                + partcodes[i] + partpaths[i] + mode + stance + ".gif";

                        }
                        catch (System.IO.IOException e)
                        {
                            partstrings[i] = "/d2c;component\\char\\" + d2class + "\\" + partcodes[i] + "\\" + d2class
                                + partcodes[i] + partpaths[i] + mode + backupstance[classId] + ".gif";
                        }
                    }
                }
                catch (NullReferenceException e)
                {

                }
            }
            
        }

        /// <summary>
        /// Adjusts the positions of the various parts of the character. Generally only needed when switching classes.
        /// </summary>
        public void fixPos()
        {
            for (int i = 0; i < partcodes.Count; i++)
            {
                posLeft[i] = partproperties[classId, i, 0];
                posTop[i] = partproperties[classId, i, 1];
                posZ[i] = partproperties[classId, i, 2];
            }
        }

        /// <summary>
        /// Sets the character's d2 class
        /// </summary>
        /// <param name="type"></param>
        public void setClass(int type)
        {
            classId = type;
            switch (type)
            {
                case 0:
                    d2class = "am";
                    break;
                case 1:
                    d2class = "ne";
                    break;
                case 2:
                    d2class = "ba";
                    break;
                case 3:
                    d2class = "so";
                    break;
                default:
                    d2class = "pa";
                    break;
            }
            if (type > 4)
            {
                setOverride(type - 5);
            }
            else
            {
                overRide = false;
                overrideVis = Visibility.Hidden;
                d2CharVis = Visibility.Visible;
                refreshAnimation();
                fixPos();
            }
        }

        /// <summary>
        /// A preset is a set of equipment appearance values that match up with a given piece of armor in the actual game.
        /// </summary>
        /// <param name="type"></param>
        public void setPreset(int type)
        {
            int[] presetValues;
            armorId = type;
            switch (type)
            {
                case 1:     //Quilted
                    presetValues = new int[6] { 0, 0, 0, 1, 1, 0 };
                    break;
                case 2:     //Leather
                    presetValues = new int[6] { 0, 0, 0, 1, 1, 1 };
                    break;
                case 3:     //Hard Leather
                    presetValues = new int[6] { 1, 0, 1, 1, 1, 1 };
                    break;
                case 4:     //Studded Leather
                    presetValues = new int[6] { 0, 1, 1, 1, 1, 0 };
                    break;
                case 5:     //Ring Mail
                    presetValues = new int[6] { 0, 1, 0, 1, 1, 1 };
                    break;
                case 6:     //Scale Mail
                    presetValues = new int[6] { 1, 1, 1, 1, 1, 1 };
                    break;
                default:    //None
                    presetValues = new int[6] {0,0,0,0,0,0};
                    break;
            }
            setLA(presetValues[0]);
            setLG(presetValues[1]);
            setRA(presetValues[2]);
            setS1(presetValues[3]);
            setS2(presetValues[4]);
            setTR(presetValues[5]);
        }

        /// <summary>
        /// Overriding means we're not bothering with assembling a d2 character, instead going for one of the 1-piece gifs.
        /// </summary>
        /// <param name="type"></param>
        public void setOverride(int type)
        {
            overrideString = "/d2c;component\\char\\or\\" + orPaths[type] + ".gif";
            overridePos[0] = overrideLeft[type];
            overridePos[1] = overrideTop[type];
            overRide = true;
            overrideVis = Visibility.Visible;
            d2CharVis = Visibility.Hidden;
        }

        public void setHD(int type)
        {
            switch (type)
            {
                case 0:
                    partpaths[0] = "lit";
                    break;
                default:
                    partpaths[0] = "cap";
                    break;
            }
            refreshAnimation();
        }

        public void setLA(int type)
        {
            switch (type)
            {
                case 0:
                    partpaths[1] = "lit";
                    break;
                default:
                    partpaths[1] = "med";
                    break;
            }
            refreshAnimation();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="type"></param>
        public void setRH(int type)
        {
            /*switch (type)
            {
                case 0:
                    rh.Visibility = Visibility.Hidden;
                    break;
                default:
                    rh.Visibility = Visibility.Visible;
                    break;
            }*/
        }

        public void setLG(int type)
        {
            switch (type)
            {
                case 0:
                    partpaths[2] = "lit";
                    break;
                default:
                    partpaths[2] = "med";
                    break;
            }
            refreshAnimation();
        }

        public void setRA(int type)
        {
            switch (type)
            {
                case 0:
                    partpaths[3] = "lit";
                    break;
                default:
                    partpaths[3] = "med";
                    break;
            }
            refreshAnimation();
        }

        public void setS1(int type)
        {
            switch (type)
            {
                case 0:
                    partpaths[5] = "lit";
                    break;
                default:
                    partpaths[5] = "med";
                    break;
            }
            refreshAnimation();
        }

        public void setS2(int type)
        {
            switch (type)
            {
                case 0:
                    partpaths[6] = "lit";
                    break;
                default:
                    partpaths[6] = "med";
                    break;
            }
            refreshAnimation();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="type"></param>
        public void setSH(int type)
        {

            /*switch (type)
            {
                case 0:
                    sh.Visibility = Visibility.Hidden;
                    break;
                default:
                    sh.Visibility = Visibility.Visible;
                    break;
            }*/
        }

        public void setTR(int type)
        {
            switch (type)
            {
                case 0:
                    partpaths[8] = "lit";
                    break;
                default:
                    partpaths[8] = "med";
                    break;
            }
            refreshAnimation();
        }

        /// <summary>
        /// Makes the 'squelched' animated gif visible.
        /// </summary>
        public void squelch()
        {
            squelched = Visibility.Visible;
        }

        /// <summary>
        /// Makes the 'squelched' animated gif hidden.
        /// </summary>
        public void unsquelch()
        {
            squelched = Visibility.Hidden;
        }

        /// <summary>
        /// Checks if the user is squelched.
        /// </summary>
        /// <returns>true if the 'squelched' animated gif is visible.</returns>
        public bool isSquelched()
        {
            return squelched == Visibility.Visible;
        }

        /// <summary>
        /// Check if the user is an operator.
        /// </summary>
        /// <returns>True if the Operator avatar is set.</returns>
        public bool isOperator()
        {
            return (overRide && overrideString.Equals("/d2c;component\\char\\or\\OPER.gif"));
        }


        /// <summary>
        /// Constructor to create an avatar of the sepecified class and equipment, and optional squelch graphic.
        /// </summary>
        /// <param name="d2class"></param>
        /// <param name="d2armor"></param>
        /// <param name="squelched"></param>
        /// <param name="name"></param>
        public d2avatar(int d2class, int d2armor, bool squelched, string name)
        {
            setup();
            setClass(d2class);
            if (d2class < 5)
            {
                setPreset(d2armor);
            }
            if (squelched)
            {
                squelch();
            }
            Name = name;
        }


        /// <summary>
        /// Constructor to create an avatar of the sepecified class, and optional squelch graphic.
        /// </summary>
        /// <param name="d2class"></param>
        /// <param name="squelched"></param>
        /// <param name="name"></param>
        public d2avatar(int d2class, bool squelched, string name)
        {
            setup();
            setClass(d2class);
            if (squelched)
            {
                squelch();
            }
            Name = name;
        }

        /// <summary>
        /// Constructor to create an avatar of the sepecified class and equipment.
        /// </summary>
        /// <param name="d2class"></param>
        /// <param name="d2armor"></param>
        /// <param name="name"></param>
        public d2avatar(int d2class, int d2armor, string name)
        {
            setup();
            setClass(d2class);
            if (d2class < 5)
            {
                setPreset(d2armor);
            }
            Name = name;
        }

        /// <summary>
        /// Constructor to create an avatar of the sepecified class and equipment.
        /// </summary>
        /// <param name="d2class"></param>
        /// <param name="d2armor"></param>
        public d2avatar(int d2class, int d2armor)
        {
            setup();
            setClass(d2class);
            if (d2class < 5)
            {
                setPreset(d2armor);
            }
        }

        /// <summary>
        /// Constructor to create an avatar of the sepecified class.
        /// </summary>
        /// <param name="d2class"></param>
        /// <param name="name"></param>
        public d2avatar(int d2class, string name)
        {
            setup();
            setClass(d2class);
            Name = name;
        }
    }
}
