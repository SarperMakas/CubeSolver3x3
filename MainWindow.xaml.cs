using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace CubeSolver {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        #region DEFINE VARS
        // Colors of the 
        private Dictionary<string, string> CodeToName = new Dictionary<string, string>() {
            {"#FFFBFDF2", "white"}, {"#FFF9FE6C", "yellow"},
            {"#FFFC0A2D", "red" }, {"#FFFD5E35", "orange"},
            {"#FF00D75A", "green"}, {"#FF0059B9", "blue" }
        };

        private Dictionary<string, string> NameToCode = new Dictionary<string, string>() {
            {"white", "#FFFBFDF2"}, {"yellow", "#FFF9FE6C"},
            {"red", "#FFFC0A2D"}, {"orange", "#FFFD5E35"},
            {"green", "#FF00D75A"}, {"blue" , "#FF0059B9"}
        };

        private Dictionary<int, string> NumberToCode = new Dictionary<int, string>() {
            {0, "#FFFBFDF2"}, {1, "#FFF9FE6C"},
            {2, "#FFFC0A2D"}, {3, "#FFFD5E35"},
            {4, "#FF00D75A"}, {5, "#FF0059B9"}
        };

        private Dictionary<string, int> CodeToNumber = new Dictionary<string, int>() {
            {"#FFFBFDF2", 0}, {"#FFF9FE6C", 1},
            {"#FFFC0A2D", 2}, {"#FFFD5E35", 3},
            {"#FF00D75A", 4}, {"#FF0059B9", 5}
        };

        private Dictionary<string, string> AntiColors = new Dictionary<string, string>() {
            {"white", "yellow" }, { "red", "orange"},
            { "blue", "green" }, { "green", "blue" },
            { "yellow", "white" }, { "orange", "red" }
        };

        private Dictionary<string, string> NextColors = new Dictionary<string, string>() {
            { "blue", "orange" }, { "orange", "green" },
            { "green", "red" }, { "red", "blue" },
        };

        // faces of the cubes
        private string[,] backFace;
        private string[,] frontFace;
        private string[,] upFace;
        private string[,] downFace;
        private string[,] rightFace;
        private string[,] leftFace;

        string commands = "";

        private Edge FU_Edge, BU_Edge, RU_Edge, LU_Edge, FD_Edge, BD_Edge, RD_Edge, LD_Edge, FRM_Edge, FLM_Edge, BRM_Edge, BLM_Edge;
        private Corner FUR_Corner, FUL_Corner, FDL_Corner, FDR_Corner, BUR_Corner, BUL_Corner, BDR_Corner, BDL_Corner;

        #endregion

        public MainWindow() {
            InitializeComponent();
        }
        private void ChangeColor(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString((NumberToCode[(CodeToNumber[button.Background.ToString()] + 1) % 6])));
        }

        private void SolveCube(object sender, RoutedEventArgs e) {
            DefineFaces();

            // first layer start --------------------------
            WhiteEdgesToTop();
            PutUpWhiteParts();
            PutSideWhiteParts();
            DoneFirstLayerCorners();
            DoneFirstLayerCorners2();
            // first layer finish -------------------------

            // second layer start -------------------------
            SecondLayerUp();
            SecondLayerMiddle();
            // second layer finish ------------------------

            // third layer start --------------------------
            MakePlus();
            MakeUpEdges();
            CorrectCorners();
            FinishCube();
            // third layer finish -------------------------

            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(commands)));
            commands = "";
        }

        private void SetColorsOfBlocks() {
            BackFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[0, 0]]));
            BackFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[0, 1]]));
            BackFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[0, 2]]));
            BackFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[1, 0]]));
            BackFaceBlock5.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[1, 1]]));
            BackFaceBlock6.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[1, 2]]));
            BackFaceBlock7.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[2, 0]]));
            BackFaceBlock8.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[2, 1]]));
            BackFaceBlock9.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[2, 2]]));

            UpFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[0, 0]]));
            UpFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[0, 1]]));
            UpFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[0, 2]]));
            UpFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[1, 0]]));
            UpFaceBlock5.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[1, 1]]));
            UpFaceBlock6.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[1, 2]]));
            UpFaceBlock7.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[2, 0]]));
            UpFaceBlock8.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[2, 1]]));
            UpFaceBlock9.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[2, 2]]));

            FrontFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[0, 0]]));
            FrontFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[0, 1]]));
            FrontFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[0, 2]]));
            FrontFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[1, 0]]));
            FrontFaceBlock5.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[1, 1]]));
            FrontFaceBlock6.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[1, 2]]));
            FrontFaceBlock7.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[2, 0]]));
            FrontFaceBlock8.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[2, 1]]));
            FrontFaceBlock9.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[2, 2]]));

            DownFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[0, 0]]));
            DownFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[0, 1]]));
            DownFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[0, 2]]));
            DownFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[1, 0]]));
            DownFaceBlock5.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[1, 1]]));
            DownFaceBlock6.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[1, 2]]));
            DownFaceBlock7.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[2, 0]]));
            DownFaceBlock8.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[2, 1]]));
            DownFaceBlock9.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[2, 2]]));

            RightFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[0, 0]]));
            RightFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[0, 1]]));
            RightFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[0, 2]]));
            RightFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[1, 0]]));
            RightFaceBlock5.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[1, 1]]));
            RightFaceBlock6.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[1, 2]]));
            RightFaceBlock7.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[2, 0]]));
            RightFaceBlock8.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[2, 1]]));
            RightFaceBlock9.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rightFace[2, 2]]));

            LeftFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[0, 0]]));
            LeftFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[0, 1]]));
            LeftFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[0, 2]]));
            LeftFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[1, 0]]));
            LeftFaceBlock5.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[1, 1]]));
            LeftFaceBlock6.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[1, 2]]));
            LeftFaceBlock7.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[2, 0]]));
            LeftFaceBlock8.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[2, 1]]));
            LeftFaceBlock9.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[2, 2]]));

        }

        private void DefineFaces() {
            backFace = new string[3, 3] {
                { CodeToName[BackFaceBlock1.Background.ToString()], CodeToName[BackFaceBlock2.Background.ToString()], CodeToName[BackFaceBlock3.Background.ToString()] },
                { CodeToName[BackFaceBlock4.Background.ToString()], CodeToName[BackFaceBlock5.Background.ToString()], CodeToName[BackFaceBlock6.Background.ToString()] },
                { CodeToName[BackFaceBlock7.Background.ToString()], CodeToName[BackFaceBlock8.Background.ToString()], CodeToName[BackFaceBlock9.Background.ToString()] }
            };
            upFace = new string[3, 3] {
                { CodeToName[UpFaceBlock1.Background.ToString()], CodeToName[UpFaceBlock2.Background.ToString()], CodeToName[UpFaceBlock3.Background.ToString()] },
                { CodeToName[UpFaceBlock4.Background.ToString()], CodeToName[UpFaceBlock5.Background.ToString()], CodeToName[UpFaceBlock6.Background.ToString()] },
                { CodeToName[UpFaceBlock7.Background.ToString()], CodeToName[UpFaceBlock8.Background.ToString()], CodeToName[UpFaceBlock9.Background.ToString()] }
            };
            frontFace = new string[3, 3] {
                { CodeToName[FrontFaceBlock1.Background.ToString()], CodeToName[FrontFaceBlock2.Background.ToString()], CodeToName[FrontFaceBlock3.Background.ToString()] },
                { CodeToName[FrontFaceBlock4.Background.ToString()], CodeToName[FrontFaceBlock5.Background.ToString()], CodeToName[FrontFaceBlock6.Background.ToString()] },
                { CodeToName[FrontFaceBlock7.Background.ToString()], CodeToName[FrontFaceBlock8.Background.ToString()], CodeToName[FrontFaceBlock9.Background.ToString()] }
            };
            downFace = new string[3, 3] {
                { CodeToName[DownFaceBlock1.Background.ToString()], CodeToName[DownFaceBlock2.Background.ToString()], CodeToName[DownFaceBlock3.Background.ToString()] },
                { CodeToName[DownFaceBlock4.Background.ToString()], CodeToName[DownFaceBlock5.Background.ToString()], CodeToName[DownFaceBlock6.Background.ToString()] },
                { CodeToName[DownFaceBlock7.Background.ToString()], CodeToName[DownFaceBlock8.Background.ToString()], CodeToName[DownFaceBlock9.Background.ToString()] }
            };
            rightFace = new string[3, 3] {
                { CodeToName[RightFaceBlock1.Background.ToString()], CodeToName[RightFaceBlock2.Background.ToString()], CodeToName[RightFaceBlock3.Background.ToString()] },
                { CodeToName[RightFaceBlock4.Background.ToString()], CodeToName[RightFaceBlock5.Background.ToString()], CodeToName[RightFaceBlock6.Background.ToString()] },
                { CodeToName[RightFaceBlock7.Background.ToString()], CodeToName[RightFaceBlock8.Background.ToString()], CodeToName[RightFaceBlock9.Background.ToString()] }
            };
            leftFace = new string[3, 3] {
                { CodeToName[LeftFaceBlock1.Background.ToString()], CodeToName[LeftFaceBlock2.Background.ToString()], CodeToName[LeftFaceBlock3.Background.ToString()] },
                { CodeToName[LeftFaceBlock4.Background.ToString()], CodeToName[LeftFaceBlock5.Background.ToString()], CodeToName[LeftFaceBlock6.Background.ToString()] },
                { CodeToName[LeftFaceBlock7.Background.ToString()], CodeToName[LeftFaceBlock8.Background.ToString()], CodeToName[LeftFaceBlock9.Background.ToString()] }
            };
        }
        
        private void DefineCornersAndEdges() {
            // define corners F => front B => back D => down U => up R => right L => left
            // fb ud rl

            // front corners

            FUR_Corner = new Corner(frontFace[0, 2], upFace[2, 2], rightFace[0, 0]);
            FUL_Corner = new Corner(frontFace[0, 0], upFace[2, 0], leftFace[0, 2]);
            FDL_Corner = new Corner(frontFace[2, 0], downFace[0, 0], leftFace[2, 2]);
            FDR_Corner = new Corner(frontFace[2, 2], downFace[0, 2], rightFace[2, 0]);
            // back Corners
            BUR_Corner = new Corner(backFace[2, 2], upFace[0, 2], rightFace[0, 2]);
            BUL_Corner = new Corner(backFace[2, 0], upFace[0, 0], leftFace[0, 0]);
            BDR_Corner = new Corner(backFace[0, 2], downFace[2, 2], rightFace[2, 2]);
            BDL_Corner = new Corner(backFace[0, 0], downFace[2, 0], leftFace[2, 0]);


            // up edges  ud rl fb
            FU_Edge = new Edge(upFace[2, 1], "", frontFace[0, 1]);
            BU_Edge = new Edge(upFace[0, 1], "", backFace[2, 1]);

            RU_Edge = new Edge(upFace[1, 2], rightFace[0, 1], "");
            LU_Edge = new Edge(upFace[1, 0], leftFace[0, 1], "");

            FD_Edge = new Edge(downFace[0, 1], "", frontFace[2, 1]);
            BD_Edge = new Edge(downFace[2, 1], "", backFace[0, 1]);

            RD_Edge = new Edge(downFace[1, 2], rightFace[2, 1], "");
            LD_Edge = new Edge(downFace[1, 0], leftFace[2, 1], "");

            FRM_Edge = new Edge("", rightFace[1, 0], frontFace[1, 2]);
            FLM_Edge = new Edge("", leftFace[1, 2], frontFace[1, 0]);
            BRM_Edge = new Edge("", rightFace[1, 2], backFace[1, 2]);
            BLM_Edge = new Edge("", leftFace[1, 0], backFace[1, 0]);
        }

        #region MOVEMENTS
        #region RIGTH MOVEMENTS
        private void R(bool add=true) {

            // Turn Rigth Faces
            rightFace = new string[3, 3] {
                { rightFace[2,0], rightFace[1,0], rightFace[0,0]},
                { rightFace[2,1], rightFace[1,1], rightFace[0,1]},
                { rightFace[2,2], rightFace[1,2], rightFace[0,2]}
            };

            string[] upRigth = new string[]    { upFace[0, 2], upFace[1, 2], upFace[2, 2] };
            string[] downRigth = new string[]  { downFace[0, 2], downFace[1, 2], downFace[2, 2] };
            string[] frontRigth = new string[] { frontFace[0, 2], frontFace[1, 2], frontFace[2, 2]  };
            string[] backRigth = new string[]  { backFace[0, 2], backFace[1, 2], backFace[2, 2] } ;
            
            // Turn Rigth
            backFace[0,2] = upRigth[0];     backFace[1,2] = upRigth[1];     backFace[2,2] = upRigth[2];
            upFace[0, 2] = frontRigth[0];   upFace[1, 2] = frontRigth[1];   upFace[2, 2] = frontRigth[2];
            frontFace[0, 2] = downRigth[0]; frontFace[1, 2] = downRigth[1]; frontFace[2, 2] = downRigth[2];
            downFace[0, 2] = backRigth[0];  downFace[1, 2] = backRigth[1];  downFace[2, 2] = backRigth[2];
            
            SetColorsOfBlocks();

            if (add)
                commands += "R ";
        }

        private void Ri() {
            R(false); R(false); R(false);
            commands += "Ri ";
        }
        #endregion

        #region LEFT MOVEMENTS

        private void L(bool add=true) {

            // Turn Rigth Faces
            leftFace = new string[3, 3] {
                { leftFace[2,0], leftFace[1,0], leftFace[0,0]},
                { leftFace[2,1], leftFace[1,1], leftFace[0,1]},
                { leftFace[2,2], leftFace[1,2], leftFace[0,2]}
            };

            string[] downLeft = new string[3] { downFace[0, 0], downFace[1, 0], downFace[2, 0] };
            string[] backLeft = new string[3] { backFace[0, 0], backFace[1, 0], backFace[2, 0] };
            string[] upLeft = new string[3] { upFace[0, 0], upFace[1, 0], upFace[2, 0] };
            string[] frontLeft = new string[3] { frontFace[0, 0], frontFace[1, 0], frontFace[2, 0] };

            // Turn Rigth
            backFace[0, 0] = downLeft[0];  backFace[1, 0] = downLeft[1];  backFace[2, 0] = downLeft[2];
            upFace[0, 0] = backLeft[0];    upFace[1, 0] = backLeft[1];    upFace[2, 0] = backLeft[2];
            frontFace[0, 0] = upLeft[0];   frontFace[1, 0] = upLeft[1];   frontFace[2, 0] = upLeft[2];
            downFace[0, 0] = frontLeft[0]; downFace[1, 0] = frontLeft[1]; downFace[2, 0] = frontLeft[2];

            SetColorsOfBlocks();
            if (add)
                commands += "L ";

        }

        private void Li(bool a = true) {
            L(false); L(false); L(false);
            commands += "Li ";
        }
        #endregion

        #region FRONT MOVEMENTS
        private void F(bool add=true) {

            // create new front face
            frontFace = new string[3, 3] {
                { frontFace[2, 0], frontFace[1, 0], frontFace[0, 0] },
                { frontFace[2, 1], frontFace[1, 1], frontFace[0, 1] },
                { frontFace[2, 2], frontFace[1, 2], frontFace[0, 2] }
            };

            // get data
            string[] upFront = new string[] { upFace[2, 0], upFace[2, 1], upFace[2, 2] };
            string[] rightFront = new string[] { rightFace[0, 0], rightFace[1, 0], rightFace[2, 0] };
            string[] leftFront = new string[] { leftFace[0, 2], leftFace[1, 2], leftFace[2, 2] };
            string[] downFront = new string[] { downFace[0, 0], downFace[0, 1], downFace[0, 2] };

            // put the datas
            upFace[2, 0] = leftFront[2]; upFace[2, 1] = leftFront[1]; upFace[2, 2] = leftFront[0];
            rightFace[0, 0] = upFront[0]; rightFace[1, 0] = upFront[1]; rightFace[2, 0] = upFront[2];
            downFace[0, 0] = rightFront[2]; downFace[0, 1] = rightFront[1]; downFace[0, 2] = rightFront[0];
            leftFace[0, 2] = downFront[0]; leftFace[1, 2] = downFront[1]; leftFace[2, 2] = downFront[2];

            SetColorsOfBlocks();
            if (add)
                commands += "F ";
        }

        private void Fi() {
            F(false); F(false); F(false);
            commands += "Fi ";
        }
        #endregion

        #region BACK MOVEMENTS
        private void B(bool add=true) {

            // turn face
            backFace = new string[3, 3] {
                { backFace[0, 2], backFace[1, 2], backFace[2, 2] },
                { backFace[0, 1], backFace[1, 1], backFace[2, 1] },
                { backFace[0, 0], backFace[1, 0], backFace[2, 0] }
            };

            // turn other parts
            string[] upBack = new string[] { upFace[0, 0], upFace[0, 1], upFace[0, 2] };
            string[] rightBack = new string[] { rightFace[0, 2], rightFace[1, 2], rightFace[2, 2] };
            string[] leftBack = new string[] { leftFace[0, 0], leftFace[1, 0], leftFace[2, 0] }; 
            string[] downBack = new string[] { downFace[2, 0], downFace[2, 1], downFace[2, 2] };

            upFace[0, 0] = leftBack[2]; upFace[0, 1] = leftBack[1]; upFace[0, 2] = leftBack[0];
            rightFace[0, 2] = upBack[0]; rightFace[1, 2] = upBack[1]; rightFace[2, 2] = upBack[2];
            downFace[2, 0] = rightBack[2]; downFace[2, 1] = rightBack[1]; downFace[2,2] = rightBack[0];
            leftFace[0, 0] = downBack[0]; leftFace[1, 0] = downBack[1]; leftFace[2, 0] = downBack[2];

            SetColorsOfBlocks();
            if (add)
                commands += "B ";
        }

        private void Bi() {
            B(false); B(false); B(false);
            commands += "Bi ";
        }
        #endregion

        #region E MOVEMENT
        private void E(bool add=true) {

            // data
            string[] frontE = new string[] { frontFace[1, 0], frontFace[1, 1], frontFace[1, 2] };
            string[] rightE = new string[] { rightFace[1, 0], rightFace[1, 1], rightFace[1, 2] };
            string[] leftE = new string[] { leftFace[1, 0], leftFace[1, 1], leftFace[1, 2] };
            string[] backE = new string[] { backFace[1, 0], backFace[1, 1], backFace[1, 2] };

            frontFace[1, 0] = leftE[0]; frontFace[1, 1] = leftE[1]; frontFace[1, 2] = leftE[2];
            rightFace[1, 0] = frontE[0]; rightFace[1, 1] = frontE[1]; rightFace[1, 2] = frontE[2];
            backFace[1, 0] = rightE[2]; backFace[1, 1] = rightE[1]; backFace[1, 2] = rightE[0];
            leftFace[1, 0] = backE[2]; leftFace[1, 1] = backE[1]; leftFace[1, 2] = backE[0];

            SetColorsOfBlocks();
            if (add)
                commands += "E ";
        }

        private void Ei() {
            E(); E(); E();
            commands += "Ei ";
        }
        #endregion

        #region DOWN MOVEMENT
        private void D(bool add = true) {

            // turn face
            downFace = new string[3, 3] {
                { downFace[2, 0], downFace[1, 0], downFace[0, 0] },
                { downFace[2, 1], downFace[1, 1], downFace[0, 1] },
                { downFace[2, 2], downFace[1, 2], downFace[0, 2] }
            };

            // get data
            string[] frontDown = new string[] { frontFace[2, 0], frontFace[2, 1], frontFace[2, 2] };
            string[] rightDown = new string[] { rightFace[2, 0], rightFace[2, 1], rightFace[2, 2] };
            string[] backDown = new string[] { backFace[0, 0], backFace[0, 1], backFace[0, 2] };
            string[] leftDown = new string[] { leftFace[2, 0], leftFace[2, 1], leftFace[2, 2] };

            frontFace[2, 0] = leftDown[0]; frontFace[2, 1] = leftDown[1]; frontFace[2, 2] = leftDown[2];
            rightFace[2, 0] = frontDown[0]; rightFace[2, 1] = frontDown[1]; rightFace[2, 2] = frontDown[2];
            backFace[0, 0] = rightDown[2]; backFace[0, 1] = rightDown[1]; backFace[0, 2] = rightDown[0];
            leftFace[2, 0] = backDown[2]; leftFace[2, 1] = backDown[1]; leftFace[2, 2] = backDown[0];

            SetColorsOfBlocks();
            if (add)
                commands += "D ";
        }
        private void Di() {
            D(); D(); D();
            commands += "Di ";
        }

        private void d(bool add = true) {
            D(false); E(false);
            if (add)
                commands += "d ";
        }
        private void di() {
            d(false); d(false); d(false);
            commands += "di ";
        }
        #endregion

        #region UP MOVEMENT
        private void U(bool add = true) {

            // define face
            upFace = new string[,] {
                { upFace[2, 0], upFace[1, 0], upFace[0, 0] },
                { upFace[2, 1], upFace[1, 1], upFace[0, 1] },
                { upFace[2, 2], upFace[1, 2], upFace[0, 2] }
            };

            // data
            string[] frontUp = new string[] { frontFace[0, 0], frontFace[0, 1], frontFace[0, 2] };
            string[] rightUp = new string[] { rightFace[0, 0], rightFace[0, 1], rightFace[0, 2] };
            string[] leftUp = new string[] { leftFace[0, 0], leftFace[0, 1], leftFace[0, 2] };
            string[] backUp = new string[] { backFace[2, 0], backFace[2, 1], backFace[2, 2] };

            frontFace[0, 0] = rightUp[0]; frontFace[0, 1] = rightUp[1]; frontFace[0, 2] = rightUp[2];
            leftFace[0, 0] = frontUp[0]; leftFace[0, 1] = frontUp[1]; leftFace[0, 2] = frontUp[2];
            rightFace[0, 0] = backUp[2]; rightFace[0, 1] = backUp[1]; rightFace[0, 2] = backUp[0];
            backFace[2, 0] = leftUp[2]; backFace[2, 1] = leftUp[1]; backFace[2, 2] = leftUp[0];
            Trace.WriteLine(backFace[2, 2]);


            SetColorsOfBlocks();
            if (add)
                commands += "U ";
        }

        private void Ui(bool a=true) {
            U(false); U(false); U(false);
            commands += "Ui ";
        }
        #endregion
        #endregion

        #region MAKE FIRST LAYER
        #region MAKING WHITE PLUS
        #region PUT THE EDGES TO THE TOP
        private void WhiteEdgesToTop() {
            DefineCornersAndEdges();
            bool run = true;
            while (run) {
                Trace.WriteLine($"State: {!RU_Edge.elements.Contains("white") || !LU_Edge.elements.Contains("white") || !FU_Edge.elements.Contains("white") || !BU_Edge.elements.Contains("white")} {!RU_Edge.elements.Contains("white")} {!LU_Edge.elements.Contains("white")} {!FU_Edge.elements.Contains("white")} {!BU_Edge.elements.Contains("white")}");
                Trace.WriteLine($"{FRM_Edge.elements.Contains("white")} 1");
                run = false;
                if (FRM_Edge.elements.Contains("white")) {
                    while (RU_Edge.elements.Contains("white")) {
                        U(); DefineCornersAndEdges();
                    }
                    R(); DefineCornersAndEdges();
                    Trace.WriteLine(commands);
                    run = true;
                }
                DefineCornersAndEdges();
                Trace.WriteLine($"{BRM_Edge.elements.Contains("white")} 2");
                if (BRM_Edge.elements.Contains("white")) {
                    while (RU_Edge.elements.Contains("white")) {
                        U(); DefineCornersAndEdges();
                    }
                    Ri(); DefineCornersAndEdges();
                    Trace.WriteLine(commands);
                    run = true;
                }
                DefineCornersAndEdges();
                Trace.WriteLine($"{FLM_Edge.elements.Contains("white")} 3");
                if (FLM_Edge.elements.Contains("white")) {
                    while (LU_Edge.elements.Contains("white")) {
                        U(); DefineCornersAndEdges();
                    }
                    Li(); DefineCornersAndEdges();
                    Trace.WriteLine(commands);
                    run = true;
                }
                DefineCornersAndEdges();
                Trace.WriteLine($"{BLM_Edge.elements.Contains("white")} 4");
                if (BLM_Edge.elements.Contains("white")) {
                    while (LU_Edge.elements.Contains("white")) {
                        U(); DefineCornersAndEdges();
                    }
                    L(); DefineCornersAndEdges();
                    Trace.WriteLine(commands);
                    run = true;
                }
                DefineCornersAndEdges();
                Trace.WriteLine($"{RD_Edge.elements.Contains("white")} 5");
                if (RD_Edge.elements.Contains("white")) {
                    while (RU_Edge.elements.Contains("white")) {
                        U(); DefineCornersAndEdges();
                    }
                    R(); R(); DefineCornersAndEdges();
                    Trace.WriteLine(commands);
                    run = true;
                }
                DefineCornersAndEdges();
                Trace.WriteLine($"{LD_Edge.elements.Contains("white")} 6");
                if (LD_Edge.elements.Contains("white")) {
                    while (LU_Edge.elements.Contains("white")) {
                        U(); DefineCornersAndEdges();
                    }
                    L(); L(); DefineCornersAndEdges();
                    Trace.WriteLine(commands);
                    run = true;
                }
                DefineCornersAndEdges();
                Trace.WriteLine($"{FD_Edge.elements.Contains("white")} 7");
                if (FD_Edge.elements.Contains("white")) {
                    while (FU_Edge.elements.Contains("white")) {
                        U(); DefineCornersAndEdges();
                    }
                    F(); F(); DefineCornersAndEdges();
                    Trace.WriteLine(commands);
                    run = true;
                }
                DefineCornersAndEdges();
                Trace.WriteLine($"{BD_Edge.elements.Contains("white")} 8");
                if (BD_Edge.elements.Contains("white")) {
                    while (BU_Edge.elements.Contains("white")) {
                        U(); DefineCornersAndEdges();
                    }
                    B(); B(); DefineCornersAndEdges();
                    Trace.WriteLine(commands);
                    run = true;
                }
                DefineCornersAndEdges();
            }
        }

        #endregion

        #region PUT THE  WHITE PARTS
        private void PutUpWhiteParts() {

            // we will do 4 times because there are 4 faces [front, back, right, left]
            while (upFace[0, 1] == "white" || upFace[1, 0] == "white" || upFace[1, 2] == "white" || upFace[2, 1] == "white"){

                Trace.WriteLine($"{upFace[0, 1] == "white"} {upFace[1, 0] == "white"} {upFace[1, 2] == "white"} {upFace[2, 1] == "white"}");
                while (upFace[2, 1] != "white") {
                    U();
                }

                // check sides
                while (frontFace[0, 1] != frontFace[1, 1])
                    d();
                // put them down
                F(); F();

                Trace.WriteLine(commands);
            }
            if (frontFace[0, 1] == "white" || rightFace[0, 1] == "white" || leftFace[0, 1] == "white" || backFace[2, 1] == "white")
                PutSideWhiteParts();
        }

        private void PutSideWhiteParts() {
            while (frontFace[0, 1] == "white" || rightFace[0, 1] == "white" || leftFace[0, 1] == "white" || backFace[2, 1] == "white") {
                Trace.WriteLine($"{frontFace[0, 1] == "white"} {rightFace[0, 1] == "white"} {leftFace[0, 1] == "white"} {backFace[2, 1] == "white"}");
                while (frontFace[0, 1] != "white") {
                    Trace.WriteLine("U");
                    U();
                }
                // check sides
                while (upFace[2, 1] != leftFace[1, 1]) {
                    Trace.WriteLine($" {rightFace[1,1]} {backFace[1, 1]} {leftFace[1, 1]} {frontFace[1, 1]}");
                    Trace.WriteLine($" {upFace[2, 1]} {leftFace[1, 1]}");
                    d();
                    Trace.WriteLine($"d {upFace[2, 1]} {leftFace[1, 1]}");
                }
                Trace.WriteLine("Fi L F");
                Fi(); L(); F();
                Trace.WriteLine(commands);
            }
            if (upFace[0, 1] == "white" || upFace[1, 0] == "white" || upFace[1, 2] == "white" || upFace[2, 1] == "white")
                PutUpWhiteParts();
        }
        #endregion
        #endregion

        #region MAKE FIRST LAYERS CORNER
        private bool In(string[] elements, string data) {
            for (int i = 0; i < elements.Length; i++) {
                // check in
                if (elements[i] == data)
                    return true;
            }
            return false;
        }

        private void DoneFirstLayerCorners() {
            DefineCornersAndEdges(); // define corners
            while (FUR_Corner.elements.Contains("white") || BUR_Corner.elements.Contains("white") || FUL_Corner.elements.Contains("white") || BUL_Corner.elements.Contains("white")) {
                // use all the corners

                while (!FUR_Corner.elements.Contains("white")) {
                    U(); // put corner to the correct place
                    DefineCornersAndEdges(); // define corners
                    Trace.WriteLine($"U {FUR_Corner.elements.Contains("white")}");
                }

                DefineCornersAndEdges(); // define corners
                while (!(In(FUR_Corner.elements, frontFace[1, 1]) && In(FUR_Corner.elements, rightFace[1, 1]))) {
                    Trace.WriteLine($"{In(FUR_Corner.elements, frontFace[1, 1])} {In(FUR_Corner.elements, rightFace[1, 1])} d");
                    d(); // get to the correct pos
                    DefineCornersAndEdges(); // define corners
                }

                DefineCornersAndEdges(); // define corners
                while (downFace[0, 2] != "white") {
                    R(); U(); Ri(); Ui();
                    Trace.WriteLine($"R U Ri Ui R U Ri Ui {downFace[0, 2]}");
                    DefineCornersAndEdges(); // define corners
                }
                DefineCornersAndEdges(); // define corners
            }
        }

        private void DoneFirstLayerCorners2() {
            while (downFace[0, 0] != "white" || downFace[0, 2] != "white" || downFace[2, 0] != "white" || downFace[2, 2] != "white") {

                // find the uncorrect piece
                while (downFace[0, 2] == "white") {
                    d();
                    Trace.WriteLine("U");
                    DefineCornersAndEdges(); // define corners
                }

                while (downFace[0, 2] != "white") {
                    R(); U(); Ri(); Ui();
                    Trace.WriteLine($"R U Ri Ui R U Ri Ui {downFace[0, 2]}");
                    DefineCornersAndEdges(); // define corners
                }
            }
        }
        #endregion
        #endregion

        #region MAKE SECOND LAYER
        private void SecondLayerUp() {
            DefineCornersAndEdges();
            while (FRM_Edge.elements.Contains("yellow") || FLM_Edge.elements.Contains("yellow") || BRM_Edge.elements.Contains("yellow") || BLM_Edge.elements.Contains("yellow")) {
                DefineCornersAndEdges();
                Trace.WriteLine($" {FRM_Edge.elements.Contains("yellow") || FLM_Edge.elements.Contains("yellow") || BRM_Edge.elements.Contains("yellow") || BLM_Edge.elements.Contains("yellow")} {FRM_Edge.elements.Contains("yellow")} {FLM_Edge.elements.Contains("yellow")} {BRM_Edge.elements.Contains("yellow")} {BLM_Edge.elements.Contains("yellow")}");
                // check if the up elements contains edge elements that we want
                if (!RU_Edge.elements.Contains("yellow") || !LU_Edge.elements.Contains("yellow") || !FU_Edge.elements.Contains("yellow") || !BU_Edge.elements.Contains("yellow")) {

                    while (FU_Edge.elements.Contains("yellow")) {
                        U(); // turn
                        Trace.WriteLine("U");
                        DefineCornersAndEdges();
                    }

                    // secronize the colors
                    while (frontFace[0, 1] != frontFace[1, 1]) {
                        Trace.WriteLine($" {frontFace[0, 1]} {frontFace[1, 1]} ");
                        d();
                        Trace.WriteLine($"d {frontFace[0, 1]} {frontFace[1, 1]} \n");
                        DefineCornersAndEdges();
                    }

                    if (upFace[2, 1] == rightFace[1, 1]) {
                        U(); R(); Ui();  Ri(); Ui(); Fi(); U(); F();
                        Trace.WriteLine("U R Ui Ri Ui Fi U F");
                    }
                    if (upFace[2, 1] == leftFace[1, 1]) {
                        Ui(); Li(); U(); L(); U(); F(); Ui(); Fi();
                        Trace.WriteLine("Ui Li U L U F Ui Fi");
                    }
                    DefineCornersAndEdges();
                }
                DefineCornersAndEdges();
            }
        }
        
        private void SecondLayerMiddle() {
            while (CheckMiddle()) {

                // check correct part
                while (frontFace[1, 1] == frontFace[1, 2] && rightFace[1, 0] == rightFace[1, 1]) {
                    d();
                    Trace.WriteLine("d ");
                }
                R(); Ui(); Ri(); Ui(); Fi(); U(); F(); // put it out
                Trace.WriteLine("R Ui Ri Ui Fi U F");
                SecondLayerUp();
                Trace.WriteLine("Second layer middle finish \n");
            }
        }

        private bool CheckMiddle() {
            // check midle edges
            bool frm = (frontFace[1, 1] == frontFace[1, 2] && rightFace[1, 0] == rightFace[1, 1]);
            bool flm = (frontFace[1, 1] == frontFace[1, 0] && leftFace[1, 1] == leftFace[1, 2]);
            bool brm = (backFace[1, 1] == backFace[1, 2] && rightFace[1, 1] == rightFace[1, 2]);
            bool blm = (backFace[1, 1] == backFace[1, 0] && leftFace[1, 1] == leftFace[1, 0]);

            return !frm || !flm || !brm || !blm;

        }
        #endregion

        #region MAKE THIRD (LAST) LAYER
        private void MakePlus() {
            while (upFace[0, 1] != "yellow" || upFace[1, 0] != "yellow" || upFace[1, 2] != "yellow" || upFace[2, 1] != "yellow") {

                for (int i = 0; i < 4; i++) {
                    if ((upFace[0, 1] == "yellow" && upFace[1, 0] == "yellow") || (upFace[1, 0] == "yellow" && upFace[1, 2] == "yellow"))
                        break;


                    U();
                }

                F(); R(); U(); Ri(); Ui(); Fi();
            }
        }

        private void MakeUpEdges() {

            // check first statement
            if (rightFace[0, 1] == AntiColors[leftFace[0, 1]]) {
                while (rightFace[0, 1] != rightFace[1, 1]) {
                    d();
                }
                R(); U(); U(); Ri(); Ui(); R(); Ui(); Ri();
                Trace.WriteLine("--------------");
            }

            bool run = true;
            while (frontFace[0, 1] != frontFace[1, 1] || rightFace[0, 1] != rightFace[1, 1] || leftFace[0, 1] != leftFace[1, 1] || backFace[1, 1] != backFace[2, 1]) { 
                // put them correct place
                while (backFace[2, 1] != NextColors[leftFace[0, 1]]) {
                    Trace.WriteLine($"{backFace[2, 1]} {leftFace[0, 1]} {NextColors[leftFace[0, 1]]}");
                    U();
                    if (frontFace[0, 1] == frontFace[1, 1] && rightFace[0, 1] == rightFace[1, 1] && leftFace[0, 1] == leftFace[1, 1] && backFace[1, 1] == backFace[2, 1]) {
                        run = false;
                    }
                    Trace.WriteLine("U");
                }
                if (run == false)
                    break;
                while (leftFace[1, 1] != leftFace[0, 1]) {
                    d();
                    Trace.WriteLine("d");
                }
                if (frontFace[0, 1] == frontFace[1, 1] && rightFace[0, 1] == rightFace[1, 1] && leftFace[0, 1] == leftFace[1, 1] && backFace[1, 1] == backFace[2, 1])
                    break;

                R(); U(); U();  Ri(); Ui(); R(); Ui(); Ri();
                Trace.WriteLine("R(); U(); U();  Ri(); Ui(); R(); Ui(); Ri();");
                Trace.WriteLine($"{frontFace[0, 1] != frontFace[1, 1]} {rightFace[0, 1] != rightFace[1, 1]} {leftFace[0, 1] != leftFace[1, 1]} {backFace[1, 1] != backFace[2, 1]}");
            }

        }

        private void CorrectCorners() {

            DefineCornersAndEdges();
            // check corners are correct
            while (!(FUL_Corner.elements.Contains(frontFace[1, 1]) && FUL_Corner.elements.Contains(leftFace[1, 1])) || !(FUR_Corner.elements.Contains(frontFace[1, 1]) && FUR_Corner.elements.Contains(rightFace[1, 1])) ||
                !(BUR_Corner.elements.Contains(rightFace[1, 1]) && BUR_Corner.elements.Contains(backFace[1, 1])) || !(BUL_Corner.elements.Contains(leftFace[1, 1]) && BUL_Corner.elements.Contains(backFace[1, 1]))) {
                Trace.WriteLine($"{FUL_Corner.elements.Contains(frontFace[1, 1])} {FUL_Corner.elements.Contains(leftFace[1, 1])} | {FUR_Corner.elements.Contains(frontFace[1, 1])}  {FUR_Corner.elements.Contains(rightFace[1, 1])} {FUR_Corner.fb} {FUR_Corner.rl} {FUR_Corner.ud} |{BUR_Corner.elements.Contains(rightFace[1, 1])}  {BUR_Corner.elements.Contains(backFace[1, 1])}  | {BUL_Corner.elements.Contains(leftFace[1, 1])}  {BUL_Corner.elements.Contains(backFace[1, 1])} ");
                
                int turnNum = 0;
                while (!(FUL_Corner.elements.Contains(frontFace[1, 1]) && FUL_Corner.elements.Contains(leftFace[1, 1]))) {
                    // turn cube
                    Ui(); d();
                    DefineCornersAndEdges();
                    Trace.WriteLine("Ui, d");

                    // there might be none of the corners are secronize
                    turnNum++;
                    if (turnNum == 4) break;
                }

                Trace.WriteLine($"{FUL_Corner.elements.Contains(frontFace[1, 1])} {FUL_Corner.elements.Contains(leftFace[1, 1])} | {FUR_Corner.elements.Contains(frontFace[1, 1])}  {FUR_Corner.elements.Contains(rightFace[1, 1])} {FUR_Corner.fb} {FUR_Corner.rl} {FUR_Corner.ud} |{BUR_Corner.elements.Contains(rightFace[1, 1])}  {BUR_Corner.elements.Contains(backFace[1, 1])}  | {BUL_Corner.elements.Contains(leftFace[1, 1])}  {BUL_Corner.elements.Contains(backFace[1, 1])} ");

                Trace.WriteLine("R Ui Li U Ri Ui L U");
                R(); Ui(); Li(); U(); Ri(); Ui(); L(); U();
                DefineCornersAndEdges();
                Trace.WriteLine($"{FUL_Corner.elements.Contains(frontFace[1, 1])} {FUL_Corner.elements.Contains(leftFace[1, 1])} | {FUR_Corner.elements.Contains(frontFace[1, 1])}  {FUR_Corner.elements.Contains(rightFace[1, 1])} {FUR_Corner.fb} {FUR_Corner.rl} {FUR_Corner.ud} |{BUR_Corner.elements.Contains(rightFace[1, 1])}  {BUR_Corner.elements.Contains(backFace[1, 1])}  | {BUL_Corner.elements.Contains(leftFace[1, 1])}  {BUL_Corner.elements.Contains(backFace[1, 1])} ");
                Trace.WriteLine("---------");
                DefineCornersAndEdges();
            }

        }

        private void FinishCube() {

            while (rightFace[0, 0] != rightFace[0, 1] || rightFace[0, 2] != rightFace[0, 1] ||
                frontFace[0, 0] != frontFace[0, 1] || frontFace[0, 2] != frontFace[0, 1] ||
                leftFace[0, 0] != leftFace[0, 1] || leftFace[0, 2] != leftFace[0, 1] ||
                backFace[2, 0] != backFace[2, 1] || backFace[2, 2] != backFace[2, 1]) {
                while (upFace[2, 2] == "yellow") {
                    U();
                    Trace.WriteLine("U");
                }

                while (upFace[2, 2] != "yellow") {
                    Ri(); Di(); R(); D();
                    Trace.WriteLine("Ri Di R D");
                }
            }

            
            while (frontFace[0, 1] != frontFace[1, 1]) {
                U();
            }
        }

        #endregion
    }

    public class Corner {
        public string rl, fb, ud;
        public string[] elements;

        public Corner(string fb, string ud, string rl) {
            this.rl = rl; this.fb = fb; this.ud = ud;
            elements = new string[] { fb, ud, rl };
        }
    }

    public class Edge {
        public string ud, rl, fb;
        public string[] elements;

        public Edge(string ud, string rl, string fb) {
            this.ud = ud; this.rl = rl; this.fb = fb;
            elements = new string[] { rl, ud, fb};
        }
    }
}
