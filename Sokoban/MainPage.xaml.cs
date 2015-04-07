using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Xml.Linq;
using System.IO;
using Sokoban.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Threading;
using Sokoban.Web.Services;
using System.ServiceModel.DomainServices.Client;
using Sokoban.DataTypes;
using Sokoban.Classes;

namespace Sokoban
{
    public partial class MainPage : Page
    {
        Storyboard storyBoard = new Storyboard();

        private const double _BlockSize = 1;

        private const double _ContentWidthtRatio = (double)15 / 19;
        private const double _ContentHeightRatio = (double)9 / 13;

        private double _ZoomFactor = 1;

        Player currentPlayer;

        List<Box> lstBoxes = new List<Box>();
        List<Wall> lstWalls = new List<Wall>();
        List<Floor> lstFloor = new List<Floor>();
        List<Target> lstTargets = new List<Target>();

        private bool _IsLevelSolved = false;

        Stack<HistoryIncident> stackHistoryIncidents = new Stack<HistoryIncident>();

        App _Application = (App)Application.Current;

        private List<string> _LstLevelsPaths = new List<string>();

        private int _LevelIndex;

        public int LevelIndex
        {
            get { return _LevelIndex; }
            set { _LevelIndex = value; }
        }

        private bool _IsAnimatingSolution = false;
        private int _AnimationIndex = 0;
        private Solution _TargetSolution = null;

        ModalPopup _ModalPopup = new ModalPopup();

        int _LevelTime = 0;
        bool _IsPlaying = false;

        Queue<Key> _DelayedKeys = new Queue<Key>();

        private static Sokoban.DataTypes.User _CurrentPlayer;

        public static Sokoban.DataTypes.User CurrentPlayer
        {
            get { return _CurrentPlayer; }
            set { _CurrentPlayer = value; }
        }

        private static EntitySet<Sokoban.DataTypes.Progress> _PlayerProgress;

        public static EntitySet<Sokoban.DataTypes.Progress> PlayerProgress
        {
            get { return MainPage._PlayerProgress; }
            set { MainPage._PlayerProgress = value; }
        }

        XElement _XLevelHeader = null;

        PageSwitcher _PageSwitcher = null;

        ConfirmEscape _ConfirmEscape = new ConfirmEscape();

        private string oobServerPath = "http://walidaly.net/sokoban/";

        public MainPage(int levelIndex)
        {
            _LevelIndex = levelIndex;

            InitializeComponent();

            try
            {
                InitializeLevels();

                storyBoard.Completed += new EventHandler(storyBoard_Completed);

                DownloadLevel(_LevelIndex);

                //Show welcome modal popup 
                _ModalPopup.SetValue(Grid.RowSpanProperty, 3);
                _ModalPopup.SetValue(Grid.ColumnSpanProperty, 3);
                _ModalPopup.Visibility = Visibility.Collapsed;
                grdLayoutRoot.Children.Add(_ModalPopup);

                if (_LevelIndex == 0)
                {
                    string modalPopupTitle = "Welcome To Sokoban";
                    string modalPopupContent = "• Use the arrow keys to control the player." + Environment.NewLine +
                                                                                                                            "• Use the mouse to move boxes automatically by clicking on any box " + Environment.NewLine +
                                                                                                                            "   then click on the target area." + Environment.NewLine +
                                                                                                                            "• Press BackSpace to undo the last step." + Environment.NewLine +
                                                                                                                            "• Press R to reset current level.";

                    ShowModalPopup(modalPopupTitle, modalPopupContent);
                }

                //Initialize the timer
                mainTimer.Begin();

                Loaded += new RoutedEventHandler(MainPage_Loaded);

                Unloaded += new RoutedEventHandler(MainPage_Unloaded);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            storyBoard.Completed -= new EventHandler(storyBoard_Completed);
            _PageSwitcher.KeyDown -= new KeyEventHandler(pageSwitcher_KeyDown);
            mainTimer.Completed -= new EventHandler(timerMain_Completed);

            lstBoxes.Clear();
            lstFloor.Clear();
            lstTargets.Clear();
            lstWalls.Clear();
            mainCanvas.Children.Clear();
            stackHistoryIncidents.Clear();
            storyBoard.Stop();
            storyBoard.Children.Clear();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _PageSwitcher = (PageSwitcher)this.Parent;
            _PageSwitcher.KeyDown += new KeyEventHandler(pageSwitcher_KeyDown);
        }

        void pageSwitcher_KeyDown(object sender, KeyEventArgs e)
        {
            if (storyBoard.GetCurrentState() != ClockState.Active)
            {
                ProcessPressedKey(e.Key);
            }
            else
            {
                //Add the pressed key to a queue to be processed after finishing the animation
                _DelayedKeys.Enqueue(e.Key);
            }
        }

        void timerMain_Completed(object sender, EventArgs e)
        {
            if (_IsPlaying)
            {
                _LevelTime++;
                tbTime.Text = "Time: " + _LevelTime.ToString();
            }
            mainTimer.Begin();
        }

        private void ShowModalPopup(string modalPopupTitle, string modalPopupContent)
        {
            _ModalPopup.lblModalPopupTitle.Content = modalPopupTitle;
            _ModalPopup.lblModalPopoupContent.Content = modalPopupContent;
            _ModalPopup.Visibility = Visibility.Visible;
        }

        private void DownloadLevel(int levelIndex)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
            if (Application.Current.IsRunningOutOfBrowser)
            {
                wc.OpenReadAsync(new Uri(oobServerPath + "Levels/" + _LstLevelsPaths[levelIndex]));
            }
            else
            {
                wc.OpenReadAsync(new Uri(_Application.DeploymentConfigurations["ServerPath"] + "Levels/" + _LstLevelsPaths[levelIndex]));
            }
        }

        private void InitializeLevels()
        {
            //Microcosmos
            _LstLevelsPaths.Add("MicroCosmos/level1.xml");
            _LstLevelsPaths.Add("MicroCosmos/level2.xml");
            _LstLevelsPaths.Add("MicroCosmos/level3.xml");
            _LstLevelsPaths.Add("MicroCosmos/level4.xml");
            _LstLevelsPaths.Add("MicroCosmos/level5.xml");
            _LstLevelsPaths.Add("MicroCosmos/level6.xml");
            _LstLevelsPaths.Add("MicroCosmos/level7.xml");
            _LstLevelsPaths.Add("MicroCosmos/level8.xml");
            _LstLevelsPaths.Add("MicroCosmos/level9.xml");
            _LstLevelsPaths.Add("MicroCosmos/level10.xml");
            _LstLevelsPaths.Add("MicroCosmos/level11.xml");
            _LstLevelsPaths.Add("MicroCosmos/level12.xml");
            _LstLevelsPaths.Add("MicroCosmos/level13.xml");
            _LstLevelsPaths.Add("MicroCosmos/level14.xml");
            _LstLevelsPaths.Add("MicroCosmos/level15.xml");
            _LstLevelsPaths.Add("MicroCosmos/level16.xml");
            _LstLevelsPaths.Add("MicroCosmos/level17.xml");
            _LstLevelsPaths.Add("MicroCosmos/level18.xml");
            _LstLevelsPaths.Add("MicroCosmos/level19.xml");
            _LstLevelsPaths.Add("MicroCosmos/level20.xml");
            _LstLevelsPaths.Add("MicroCosmos/level21.xml");
            _LstLevelsPaths.Add("MicroCosmos/level22.xml");
            _LstLevelsPaths.Add("MicroCosmos/level23.xml");
            _LstLevelsPaths.Add("MicroCosmos/level24.xml");
            _LstLevelsPaths.Add("MicroCosmos/level25.xml");
            _LstLevelsPaths.Add("MicroCosmos/level26.xml");
            _LstLevelsPaths.Add("MicroCosmos/level27.xml");
            _LstLevelsPaths.Add("MicroCosmos/level28.xml");
            _LstLevelsPaths.Add("MicroCosmos/level29.xml");
            _LstLevelsPaths.Add("MicroCosmos/level30.xml");
            _LstLevelsPaths.Add("MicroCosmos/level31.xml");
            _LstLevelsPaths.Add("MicroCosmos/level32.xml");
            _LstLevelsPaths.Add("MicroCosmos/level33.xml");
            _LstLevelsPaths.Add("MicroCosmos/level34.xml");
            _LstLevelsPaths.Add("MicroCosmos/level35.xml");
            _LstLevelsPaths.Add("MicroCosmos/level36.xml");
            _LstLevelsPaths.Add("MicroCosmos/level37.xml");
            _LstLevelsPaths.Add("MicroCosmos/level38.xml");
            _LstLevelsPaths.Add("MicroCosmos/level39.xml");
            _LstLevelsPaths.Add("MicroCosmos/level40.xml");

            //Sokohard
            _LstLevelsPaths.Add("SokoHard/sokohard1.xml");
            _LstLevelsPaths.Add("SokoHard/sokohard2.xml");
            _LstLevelsPaths.Add("SokoHard/sokohard3.xml");
            _LstLevelsPaths.Add("SokoHard/sokohard4.xml");
            _LstLevelsPaths.Add("SokoHard/sokohard5.xml");
            _LstLevelsPaths.Add("SokoHard/sokohard6.xml");
            _LstLevelsPaths.Add("SokoHard/sokohard7.xml");
            _LstLevelsPaths.Add("SokoHard/sokohard8.xml");
        }

        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
            using (Stream levelStream = e.Result)
            {
                try
                {
                    LoadLevel(levelStream);
                    SetupAnimation();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LoadLevel(Stream levelStream)
        {
            XDocument xLevel = XDocument.Load(levelStream);
            _XLevelHeader = xLevel.Element("SavedGame").Element("Level");

            SetLevelDimensions();

            _IsLevelSolved = false;

            //Set Level Name
            tbLevelName.Text = _XLevelHeader.Attribute("Id").Value;

            //Load Level objects
            IEnumerable<XElement> xLevelRows = xLevel.Descendants("L");
            double left = 0;
            double top = 0;
            bool addFloor = false;
            foreach (XElement levelRow in xLevelRows)
            {
                addFloor = false;
                foreach (char block in levelRow.Value.ToCharArray())
                {
                    switch (block.ToString())
                    {
                        case "#":
                            lstWalls.Add(AddBlock<Wall>(left, top));
                            addFloor = true;
                            break;

                        case " ":
                            if (addFloor)
                            {
                                Floor floorInstance = AddBlock<Floor>(left, top);
                                floorInstance.MouseLeftButtonUp += new MouseButtonEventHandler(floorInstance_MouseLeftButtonUp);
                                lstFloor.Add(floorInstance);
                            }
                            break;

                        case "$":
                            Floor floorUnderBlock = AddBlock<Floor>(left, top);
                            floorUnderBlock.MouseLeftButtonUp += new MouseButtonEventHandler(floorInstance_MouseLeftButtonUp);
                            lstFloor.Add(floorUnderBlock);
                            Box boxInstance = AddBlock<Box>(left, top);
                            boxInstance.MouseLeftButtonUp += new MouseButtonEventHandler(boxInstance_MouseLeftButtonUp);
                            boxInstance.SetValue(Canvas.ZIndexProperty, 100);
                            lstBoxes.Add(boxInstance);
                            break;

                        case ".":
                            Target targetInstance = AddBlock<Target>(left, top);
                            targetInstance.MouseLeftButtonUp += new MouseButtonEventHandler(floorInstance_MouseLeftButtonUp);
                            lstTargets.Add(targetInstance);
                            break;

                        case "+":
                            Target targetUnderPlayer = AddBlock<Target>(left, top);
                            targetUnderPlayer.MouseLeftButtonUp += new MouseButtonEventHandler(floorInstance_MouseLeftButtonUp);
                            lstTargets.Add(targetUnderPlayer);
                            currentPlayer = AddBlock<Player>(left, top);
                            currentPlayer.SetValue(Canvas.ZIndexProperty, 100);
                            currentPlayer.MouseLeftButtonUp += new MouseButtonEventHandler(floorInstance_MouseLeftButtonUp);
                            break;

                        case "@":
                            Floor floorUnderPlayer = AddBlock<Floor>(left, top);
                            floorUnderPlayer.MouseLeftButtonUp += new MouseButtonEventHandler(floorInstance_MouseLeftButtonUp);
                            lstFloor.Add(floorUnderPlayer);
                            currentPlayer = AddBlock<Player>(left, top);
                            currentPlayer.SetValue(Canvas.ZIndexProperty, 100);
                            currentPlayer.MouseLeftButtonUp += new MouseButtonEventHandler(floorInstance_MouseLeftButtonUp);
                            break;

                        case "*":
                            Target targetUnderBox = AddBlock<Target>(left, top);
                            targetUnderBox.MouseLeftButtonUp += new MouseButtonEventHandler(floorInstance_MouseLeftButtonUp);
                            lstTargets.Add(targetUnderBox);
                            Box boxOnTargetInstance = AddBlock<Box>(left, top);
                            boxOnTargetInstance.MouseLeftButtonUp += new MouseButtonEventHandler(boxInstance_MouseLeftButtonUp);
                            boxOnTargetInstance.SetValue(Canvas.ZIndexProperty, 100);
                            SetBoxVisibility(boxOnTargetInstance, true, false);
                            lstBoxes.Add(boxOnTargetInstance);
                            break;
                    }
                    left += _BlockSize;
                }
                top += _BlockSize;
                left = 0;
            }
        }

        private void SetLevelDimensions()
        {
            if (_XLevelHeader != null)
            {
                //Adjust level BackGround
                if (BrowserScreenInformation.ClientHeight <= BrowserScreenInformation.ClientWidth)
                {
                    grdLayoutRoot.Width = BrowserScreenInformation.ClientHeight / 1024 * 1280;
                    grdLayoutRoot.Height = BrowserScreenInformation.ClientHeight;
                }
                else
                {
                    grdLayoutRoot.Height = BrowserScreenInformation.ClientWidth / 1280 * 1024;
                    grdLayoutRoot.Width = BrowserScreenInformation.ClientWidth;
                }

                //Get Level width and height
                double levelWidth = int.Parse(_XLevelHeader.Attribute("Width").Value) * _BlockSize;
                double levelHeight = int.Parse(_XLevelHeader.Attribute("Height").Value) * _BlockSize;
                double clientHeight = grdLayoutRoot.Height * _ContentHeightRatio;
                double clientWidth = grdLayoutRoot.Width * _ContentWidthtRatio;

                double zoomFactorHeight = clientHeight / levelHeight;
                double zoomFactorWidth = clientWidth / levelWidth;

                if (zoomFactorWidth <= zoomFactorHeight)
                {
                    _ZoomFactor = zoomFactorWidth;

                    //Set main canvas left margin
                    double topMargin = (clientHeight - levelHeight * _ZoomFactor) / 2;
                    mainCanvas.SetValue(Canvas.MarginProperty, new Thickness(0, topMargin, 0, topMargin));
                }
                else
                {
                    _ZoomFactor = zoomFactorHeight;

                    //Set main canvas left margin
                    double leftMargin = (clientWidth - levelWidth * _ZoomFactor) / 2;
                    mainCanvas.SetValue(Canvas.MarginProperty, new Thickness(leftMargin, 0, leftMargin, 0));
                }
            }
        }



        void floorInstance_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_IsAnimatingSolution)
            {
                //Get the selected box if any
                Box selectedBox = lstBoxes.SingleOrDefault(b => b.IsSelected);

                if (selectedBox != null)
                {
                    IBlockPosition targetBlock = (IBlockPosition)sender;
                    if (targetBlock.GetType() == typeof(Player))
                    {
                        targetBlock = GetBlockByPosition(targetBlock.position.Left, targetBlock.position.Top);
                    }
                    DeliverBox(selectedBox, targetBlock);
                }
            }
        }

        private void DeliverBox(Box selectedBox, IBlockPosition targetBlock)
        {
            if (targetBlock.GetType() == typeof(Target) || targetBlock.GetType() == typeof(Floor))
            {
                //Get a list of target's adjacent blocks
                List<Solution> lstRepeatedSolutions = new List<Solution>();
                List<Solution> lstRecursiveSolutions = new List<Solution>();
                Solution targetSolution = null;
                Position originalBoxPosition = selectedBox.position.Clone();
                Position originalPlayerPosition = currentPlayer.position.Clone();

                lstRecursiveSolutions.AddRange(GetRecursiveSolutions(selectedBox));

                while (lstRecursiveSolutions.Count > 0)
                {
                    targetSolution = lstRecursiveSolutions.FirstOrDefault(s => (s.BoxPosition.CompareTo(targetBlock.position) > 0));

                    if (targetSolution != null)
                    {
                        break;
                    }

                    lstRepeatedSolutions.AddRange(lstRecursiveSolutions);

                    List<Solution> lstNewSolutions = new List<Solution>();
                    foreach (Solution possibleSolution in lstRecursiveSolutions)
                    {
                        selectedBox.position = possibleSolution.BoxPosition;
                        currentPlayer.position = possibleSolution.PlayerPosition;
                        List<Solution> lstAddedSolutions = GetRecursiveSolutions(selectedBox);
                        lstNewSolutions.AddRange(lstAddedSolutions);

                        //Add the possible solution player path as the root path for the recursive solution
                        foreach (Solution newSolution in lstAddedSolutions)
                        {
                            newSolution.PlayerPath.Directions.InsertRange(0, possibleSolution.PlayerPath.Directions);
                        }
                    }

                    foreach (Solution repeatedSolution in lstRepeatedSolutions)
                    {
                        List<Solution> lstSolutionsTobeRemoved = lstNewSolutions.Where(s => (s.BoxPosition.CompareTo(repeatedSolution.BoxPosition) > 0 &&
                                                                                                   s.PlayerPath.Directions.Last() == repeatedSolution.PlayerPath.Directions.Last())).ToList();

                        foreach (Solution solution in lstSolutionsTobeRemoved)
                        {
                            lstNewSolutions.Remove(solution);
                        }
                    }

                    lstRecursiveSolutions.Clear();
                    lstRecursiveSolutions.AddRange(lstNewSolutions);
                }

                selectedBox.position = originalBoxPosition;
                currentPlayer.position = originalPlayerPosition;
                SetBoxVisibility(selectedBox, selectedBox.IsOnTarget, false);

                if (targetSolution != null)
                {
                    _IsAnimatingSolution = true;
                    _AnimationIndex = 0;
                    _TargetSolution = targetSolution;
                    AnimateSolution();
                }
            }
        }

        private void AnimateSolution()
        {
            if (_TargetSolution != null)
            {
                Direction direction = _TargetSolution.PlayerPath.Directions[_AnimationIndex];

                switch (direction)
                {
                    case Direction.Left:
                        MovePlayer(currentPlayer.position.Left - _BlockSize, currentPlayer.position.Top);
                        break;
                    case Direction.Down:
                        MovePlayer(currentPlayer.position.Left, currentPlayer.position.Top + _BlockSize);
                        break;
                    case Direction.Right:
                        MovePlayer(currentPlayer.position.Left + _BlockSize, currentPlayer.position.Top);
                        break;
                    case Direction.Up:
                        MovePlayer(currentPlayer.position.Left, currentPlayer.position.Top - _BlockSize);
                        break;
                }
            }
        }

        private List<Solution> GetRecursiveSolutions(Box selectedBox)
        {
            List<Solution> lstRecursiveSolutions = new List<Solution>();
            List<BlockPath> lstAdjacentBlocks = GetAdjacentBlocksWithRelativeDirections(selectedBox);
            foreach (BlockPath blockPath in lstAdjacentBlocks)
            {
                Type adjacentBlockType = blockPath.Block.GetType();
                if (adjacentBlockType == typeof(Floor) || adjacentBlockType == typeof(Target))
                {
                    Path path = GetDirectPath(currentPlayer, blockPath.Block, new Path());
                    if (path != null)
                    {
                        //Get opposite block position relative to player position
                        Position oppositePosition = GetOppositeBlockPosition(selectedBox, blockPath.Block);

                        IBlockPosition block = GetBlockByPosition(oppositePosition.Left, oppositePosition.Top);
                        Type blockType = block.GetType();
                        if (blockType == typeof(Floor) || blockType == typeof(Target))
                        {
                            Solution solution = new Solution();
                            solution.BoxPosition = oppositePosition;
                            solution.PlayerPath = path;
                            solution.PlayerPosition = selectedBox.position.Clone();
                            Direction moveDirection = GetMoveDirection(selectedBox.position, oppositePosition);
                            solution.PlayerPath.Directions.Add(moveDirection);
                            lstRecursiveSolutions.Add(solution);
                        }
                    }
                }
            }

            return lstRecursiveSolutions;
        }

        private Position GetOppositeBlockPosition(IBlockPosition refrenceBlock, IBlockPosition targetBlock)
        {
            Position oppositePosition = new Position();
            oppositePosition.Left = 2 * refrenceBlock.position.Left - targetBlock.position.Left;
            oppositePosition.Top = 2 * refrenceBlock.position.Top - targetBlock.position.Top;
            return oppositePosition;
        }

        private Path GetDirectPath(IBlockPosition startBlock, IBlockPosition targetBlock, Path inputPath)
        {
            List<BlockPath> lstRecursiveBlocks = GetAdjacentBlocksWithRelativeDirections(startBlock);
            List<BlockPath> lstRepeatedBlocks = new List<BlockPath>();
            Path directPath = null;

            //check if the startblock is the same as the target block
            if (startBlock.position.CompareTo(targetBlock.position) > 0)
            {
                return new Path();
            }

            while (lstRecursiveBlocks.Count > 0)
            {
                //Remove walls and boxes
                List<BlockPath> lstDiscardedBlocks = lstRecursiveBlocks.Where(b => b.Block.GetType() == typeof(Box) ||
                                                                                                                                            b.Block.GetType() == typeof(Wall)).ToList();
                foreach (BlockPath block in lstDiscardedBlocks)
                {
                    lstRecursiveBlocks.Remove(block);
                }

                //Remove any blocks that already has been checked
                foreach (BlockPath block in lstRepeatedBlocks)
                {
                    BlockPath blockPath = lstRecursiveBlocks.SingleOrDefault(b => b.Block.position.CompareTo(block.Block.position) > 0);
                    if (blockPath != null)
                    {
                        lstRecursiveBlocks.Remove(blockPath);
                    }
                }
                lstRepeatedBlocks.AddRange(lstRecursiveBlocks);

                //if there are no more blocks to search then break
                if (lstRecursiveBlocks.Count == 0) break;

                //check if any of the recursive blocks is the target
                List<BlockPath> lstNewBlocks = new List<BlockPath>();
                foreach (BlockPath blockPath in lstRecursiveBlocks)
                {
                    if (blockPath.Block == targetBlock)
                    {
                        //Target found
                        directPath = blockPath.RefrencePath;
                        break;
                    }

                    List<BlockPath> lstAdjacentBlocks = GetAdjacentBlocksWithRelativeDirections(blockPath.Block);
                    //Add current path to the adjacent block as thier root path 
                    foreach (BlockPath adjacentBlockPath in lstAdjacentBlocks)
                    {
                        if (lstNewBlocks.SingleOrDefault(b => b.Block.position.CompareTo(adjacentBlockPath.Block.position) > 0) == null)
                        {
                            adjacentBlockPath.RefrencePath.Directions.InsertRange(0, blockPath.RefrencePath.Directions);
                            lstNewBlocks.Add(adjacentBlockPath);
                        }
                    }
                }

                if (directPath != null)
                {
                    break;
                }

                lstRecursiveBlocks.Clear();
                lstRecursiveBlocks.AddRange(lstNewBlocks);
            }

            return directPath;
        }

        private static Direction GetMoveDirection(Position refrencePosition, Position targetPosition)
        {
            Direction pathDirection = new Direction();
            if (targetPosition.Left != refrencePosition.Left)
            {
                if (targetPosition.Left < refrencePosition.Left)
                {
                    pathDirection = Direction.Left;
                }
                else if (targetPosition.Left > refrencePosition.Left)
                {
                    pathDirection = Direction.Right;
                }
            }
            else if (targetPosition.Top != refrencePosition.Top)
            {
                if (targetPosition.Top < refrencePosition.Top)
                {
                    pathDirection = Direction.Up;
                }
                else if (targetPosition.Top > refrencePosition.Top)
                {
                    pathDirection = Direction.Down;
                }
            }
            return pathDirection;
        }

        private List<IBlockPosition> GetAdjacentBlocks(IBlockPosition targetBlock)
        {
            List<IBlockPosition> lstAdjacentBlocks = new List<IBlockPosition>();
            IBlockPosition adjacentBlock = null;
            //Left Block
            adjacentBlock = GetBlockByPosition(targetBlock.position.Left - _BlockSize, targetBlock.position.Top);
            lstAdjacentBlocks.Add(adjacentBlock);
            //Right Block
            adjacentBlock = GetBlockByPosition(targetBlock.position.Left + _BlockSize, targetBlock.position.Top);
            lstAdjacentBlocks.Add(adjacentBlock);
            //Up Block
            adjacentBlock = GetBlockByPosition(targetBlock.position.Left, targetBlock.position.Top - _BlockSize);
            lstAdjacentBlocks.Add(adjacentBlock);
            //Down Block
            adjacentBlock = GetBlockByPosition(targetBlock.position.Left, targetBlock.position.Top + _BlockSize);
            lstAdjacentBlocks.Add(adjacentBlock);

            return lstAdjacentBlocks;
        }

        private List<BlockPath> GetAdjacentBlocksWithRelativeDirections(IBlockPosition targetBlock)
        {
            List<BlockPath> lstAdjacentBlocks = new List<BlockPath>();
            IBlockPosition adjacentBlock = null;
            BlockPath blockPath = null;
            //Left Block
            adjacentBlock = GetBlockByPosition(targetBlock.position.Left - _BlockSize, targetBlock.position.Top);
            blockPath = new BlockPath();
            blockPath.Block = adjacentBlock;
            blockPath.RefrencePath = new Path();
            blockPath.RefrencePath.Directions.Add(Direction.Left);
            lstAdjacentBlocks.Add(blockPath);
            //Right Block
            adjacentBlock = GetBlockByPosition(targetBlock.position.Left + _BlockSize, targetBlock.position.Top);
            blockPath = new BlockPath();
            blockPath.Block = adjacentBlock;
            blockPath.RefrencePath = new Path();
            blockPath.RefrencePath.Directions.Add(Direction.Right);
            lstAdjacentBlocks.Add(blockPath);
            //Up Block
            adjacentBlock = GetBlockByPosition(targetBlock.position.Left, targetBlock.position.Top - _BlockSize);
            blockPath = new BlockPath();
            blockPath.Block = adjacentBlock;
            blockPath.RefrencePath = new Path();
            blockPath.RefrencePath.Directions.Add(Direction.Up);
            lstAdjacentBlocks.Add(blockPath);
            //Down Block
            adjacentBlock = GetBlockByPosition(targetBlock.position.Left, targetBlock.position.Top + _BlockSize);
            blockPath = new BlockPath();
            blockPath.Block = adjacentBlock;
            blockPath.RefrencePath = new Path();
            blockPath.RefrencePath.Directions.Add(Direction.Down);
            lstAdjacentBlocks.Add(blockPath);

            return lstAdjacentBlocks;
        }

        void boxInstance_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Box clickedBox = (Box)sender;
            bool isSelected = clickedBox.IsSelected;

            //Deselect any other selected box if any
            Box selectedBox = lstBoxes.SingleOrDefault(b => b.IsSelected);
            if (selectedBox != null)
            {
                SetBoxVisibility(selectedBox, selectedBox.IsOnTarget, false);
                selectedBox.IsSelected = false;
            }

            //Select the clicked box
            SetBoxVisibility(clickedBox, clickedBox.IsOnTarget, isSelected ^ true);
        }

        private T AddBlock<T>(double left, double top) where T : System.Windows.UIElement, IBlockPosition, new()
        {
            T blockInstance = new T();
            mainCanvas.Children.Add(blockInstance);
            blockInstance.SetValue(Canvas.LeftProperty, left * _ZoomFactor);
            blockInstance.SetValue(Canvas.TopProperty, top * _ZoomFactor);

            blockInstance.position.Left = left;
            blockInstance.position.Top = top;

            blockInstance.SetValue(UserControl.WidthProperty, _BlockSize * _ZoomFactor);

            return blockInstance;
        }

        private void SetupAnimation()
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 150));
            storyBoard.Duration = duration;

            //Setup player TimeLines
            AddTimeLine(currentPlayer.VerticalTimeLine, duration, Canvas.TopProperty, currentPlayer);
            AddTimeLine(currentPlayer.HorizontalTimeLine, duration, Canvas.LeftProperty, currentPlayer);

            //Setup boxes TimeLines
            foreach (Box boxInstance in lstBoxes)
            {
                AddTimeLine(boxInstance.VerticalTimeLine, duration, Canvas.TopProperty, boxInstance);
                AddTimeLine(boxInstance.HorizontalTimeLine, duration, Canvas.LeftProperty, boxInstance);
            }

            if (mainCanvas.Resources["stbPlayer"] == null)
            {
                mainCanvas.Resources.Add("stbPlayer", storyBoard);
            }
        }

        private void AddTimeLine(DoubleAnimation timeLine, Duration duration, DependencyProperty property, DependencyObject targetObject)
        {
            timeLine.Duration = duration;
            CubicEase easing = new CubicEase();
            easing.EasingMode = EasingMode.EaseIn;
            timeLine.EasingFunction = easing;
            storyBoard.Children.Add(timeLine);
            Storyboard.SetTarget(timeLine, targetObject);
            Storyboard.SetTargetProperty(timeLine, new PropertyPath(property));
        }

        private void ProcessPressedKey(Key key)
        {
            if (!_IsAnimatingSolution && !_IsLevelSolved)
            {
                switch (key)
                {
                    case Key.Add:
                        _LevelIndex++;
                        if (_LevelIndex < _LstLevelsPaths.Count)
                        {
                            ResetCurrentLevel();
                            DownloadLevel(_LevelIndex);
                        }
                        else
                        {
                            _LevelIndex = _LstLevelsPaths.Count;
                        }
                        break;
                    case Key.Subtract:
                        _LevelIndex--;
                        if (_LevelIndex >= 0)
                        {
                            ResetCurrentLevel();
                            DownloadLevel(_LevelIndex);
                        }
                        else {
                            _LevelIndex = 0;
                        }
                        break;
                  
                    case Key.Left:
                        double targetLeft = currentPlayer.position.Left - (_BlockSize);
                        MovePlayer(targetLeft, currentPlayer.position.Top);
                        break;
                    case Key.Right:
                        double targetRight = currentPlayer.position.Left + (_BlockSize);
                        MovePlayer(targetRight, currentPlayer.position.Top);
                        break;
                    case Key.Up:
                        double targetTop = currentPlayer.position.Top - (_BlockSize);
                        MovePlayer(currentPlayer.position.Left, targetTop);
                        break;
                    case Key.Down:
                        double targetDown = currentPlayer.position.Top + (_BlockSize);
                        MovePlayer(currentPlayer.position.Left, targetDown);
                        break;
                    case Key.S:
                        //SolverClient solver = new SolverClient();
                        //InputLevel inputlevel = new InputLevel();
                        //inputlevel.SokobanObjectCollection = GetCurrentLevelSchema();
                        //solver.InitiateLevelSolverAsync(inputlevel);
                        //_IsSolvingLevel = true;
                        break;
                    case Key.R:
                        ResetCurrentLevel();
                        DownloadLevel(_LevelIndex);
                        break;
                    case Key.Back:
                        if (stackHistoryIncidents.Count > 0)
                        {
                            HistoryIncident lastIncident = stackHistoryIncidents.Pop();
                            Type objectType = lastIncident.HistoryObject.GetType();
                            TransformBlock(lastIncident.HistoryObject, lastIncident.position.Left, lastIncident.position.Top, false);
                            if (objectType == typeof(Box))
                            {
                                //Set the the box image based on the ground type
                                Box boxInstance = (Box)lastIncident.HistoryObject;
                                Target targetInstance = lstTargets.SingleOrDefault(t => t.position.Left == boxInstance.position.Left && t.position.Top == boxInstance.position.Top);
                                if (targetInstance == null && boxInstance.IsOnTarget)
                                {
                                    SetBoxVisibility(boxInstance, false, false);
                                }
                                else if (targetInstance != null && !boxInstance.IsOnTarget)
                                {
                                    SetBoxVisibility(boxInstance, true, false);
                                }

                                //Transform the player back
                                lastIncident = stackHistoryIncidents.Pop();
                                TransformBlock(lastIncident.HistoryObject, lastIncident.position.Left, lastIncident.position.Top, false);
                            }
                            UpdateUITextBlocks();
                        }
                        break;
                    case Key.Enter:
                        _ModalPopup.Visibility = Visibility.Collapsed;
                        break;
                    case Key.Escape:
                        _ConfirmEscape.Show();
                        _ConfirmEscape.Closed += new EventHandler(confirmEscape_Closed);
                        break;
                }
            }
        }

        void confirmEscape_Closed(object sender, EventArgs e)
        {
            if (_ConfirmEscape.DialogResult == true)
            {
                _PageSwitcher.Navigate(new Menu(false));
            }
        }

        private void MovePlayer(double Left, double top)
        {
            object block = GetBlockByPosition(Left, top);
            Type blockType = block.GetType();
            if (blockType == typeof(Floor) || blockType == typeof(Target))
            {
                TransformBlock(currentPlayer, Left, top, true);
            }
            else if (blockType == typeof(Box))
            {
                //Get position of the opposite block
                Box adjacentBox = (Box)block;
                double? oppositeLeft = null;
                double? oppositeTop = null;
                if (adjacentBox.position.Left == currentPlayer.position.Left)
                {
                    oppositeLeft = currentPlayer.position.Left;
                    if (currentPlayer.position.Top > adjacentBox.position.Top)
                    {
                        oppositeTop = adjacentBox.position.Top - (_BlockSize);
                    }
                    else if (currentPlayer.position.Top < adjacentBox.position.Top)
                    {
                        oppositeTop = adjacentBox.position.Top + (_BlockSize);
                    }
                }
                else if (adjacentBox.position.Top == currentPlayer.position.Top)
                {
                    oppositeTop = currentPlayer.position.Top;
                    if (currentPlayer.position.Left > adjacentBox.position.Left)
                    {
                        oppositeLeft = adjacentBox.position.Left - (_BlockSize);
                    }
                    else if (currentPlayer.position.Left < adjacentBox.position.Left)
                    {
                        oppositeLeft = adjacentBox.position.Left + (_BlockSize);
                    }
                }

                //Get type of the opposite block
                if (oppositeLeft.HasValue && oppositeTop.HasValue)
                {
                    Type oppositeBlockType = GetBlockByPosition(oppositeLeft.Value, oppositeTop.Value).GetType();
                    if (oppositeBlockType == typeof(Floor) || oppositeBlockType == typeof(Target))
                    {
                        TransformBlock(currentPlayer, Left, top, true);
                        TransformBlock(adjacentBox, oppositeLeft.Value, oppositeTop.Value, true);
                        if (oppositeBlockType == typeof(Target))
                        {
                            if (!adjacentBox.IsOnTarget)
                            {
                                SetBoxVisibility(adjacentBox, true, false);
                            }

                            IsLevelSolved();
                        }
                        else
                        {
                            if (adjacentBox.IsOnTarget)
                            {
                                SetBoxVisibility(adjacentBox, false, false);
                            }
                        }
                    }
                }
            }

            UpdateUITextBlocks();
            _IsPlaying = true;
        }

        private void UpdateUITextBlocks()
        {
            int movesCount = stackHistoryIncidents.Count(h => h.HistoryObject.GetType() == typeof(Player));
            int pushesCount = stackHistoryIncidents.Count(h => h.HistoryObject.GetType() == typeof(Box));

            tbMoves.Text = "Moves: " + movesCount.ToString();
            tbPushes.Text = "Pushes: " + pushesCount.ToString();
        }

        private static void SetBoxVisibility(Box adjacentBox, bool isOnTarget, bool isSelected)
        {
            if (isSelected)
            {
                string sURL = "../Images/green box.png";
                Uri imgURI = new Uri(sURL, UriKind.Relative);
                adjacentBox.imgBox.Source = new BitmapImage(imgURI);
                adjacentBox.IsSelected = true;
            }
            else if (isOnTarget)
            {
                string sURL = "../Images/BoxOnTarget.png";
                Uri imgURI = new Uri(sURL, UriKind.Relative);
                adjacentBox.imgBox.Source = new BitmapImage(imgURI);
                adjacentBox.IsOnTarget = true;
                adjacentBox.IsSelected = false;
            }
            else
            {
                string sURL = "../Images/Box.PNG";
                Uri imgURI = new Uri(sURL, UriKind.Relative);
                adjacentBox.imgBox.Source = new BitmapImage(imgURI);
                adjacentBox.IsOnTarget = false;
                adjacentBox.IsSelected = false;
            }
        }

        private bool IsLevelSolved()
        {
            Box boxInstance = lstBoxes.FirstOrDefault(b => !b.IsOnTarget);
            if (boxInstance != null)
            {
                _IsLevelSolved = false;
            }
            else
            {
                _IsLevelSolved = true;
            }
            return _IsLevelSolved;
        }

        private void TransformBlock(IBlockAnimation block, double Left, double top, bool addHistory)
        {
            //Add History incident
            if (addHistory)
            {
                HistoryIncident incident = new HistoryIncident();
                incident.HistoryObject = block;
                incident.position.Top = block.position.Top;
                incident.position.Left = block.position.Left;
                stackHistoryIncidents.Push(incident);
            }

            if (block.position.Left != Left)
            {
                block.HorizontalTimeLine.To = Left * _ZoomFactor;
                block.position.Left = Left;
            }
            else if (block.position.Top != top)
            {
                block.VerticalTimeLine.To = top * _ZoomFactor;
                block.position.Top = top;
            }

            storyBoard.Begin();
        }

        void storyBoard_Completed(object sender, EventArgs e)
        {
            if (_IsLevelSolved)
            {
                //Save current progress for logged in players
                Progress currentProgress = null;
                if (PlayerProgress != null)
                {
                    currentProgress = PlayerProgress.SingleOrDefault(p => p.PackageId == 1 && p.LevelIndex == _LevelIndex);
                }

                int movesCount = stackHistoryIncidents.Count(h => h.HistoryObject.GetType() == typeof(Player));
                int pushesCount = stackHistoryIncidents.Count(h => h.HistoryObject.GetType() == typeof(Box));

                if (currentProgress != null)
                {
                    activitySavingProgress.IsActive = true;
                    //Overwrite the previously saved progress
                    currentProgress.Moves = movesCount;
                    currentProgress.Pushes = pushesCount;
                    currentProgress.Time = _LevelTime;

                    SubmitOperation submitProgressOperation = StaticFields.SokobanContext.SubmitChanges();
                    submitProgressOperation.Completed += new EventHandler(submitProgressOperation_Completed);
                }
                else if (CurrentPlayer != null)
                {
                    activitySavingProgress.IsActive = true;

                    //Add new progress
                    Progress newProgress = new Progress();
                    newProgress.LevelIndex = _LevelIndex;
                    newProgress.Moves = movesCount;
                    newProgress.PackageId = 1;
                    newProgress.UserId = CurrentPlayer.Id;
                    newProgress.Pushes = pushesCount;
                    newProgress.Time = _LevelTime;

                    //Update user score
                    CurrentPlayer.Score = (_LevelIndex + 1) * 100 + (1000 / (_LevelTime + movesCount + pushesCount));

                    StaticFields.SokobanContext.Progresses.Add(newProgress);
                    SubmitOperation submitProgressOperation = StaticFields.SokobanContext.SubmitChanges();
                    submitProgressOperation.Completed += new EventHandler(submitProgressOperation_Completed);
                }
                else
                {
                    ShowLevelCompletedPopup();
                }

                //Stop animating the solution if there are more moves to animate
                _TargetSolution = null;
                _IsAnimatingSolution = false;
            }

            if (_TargetSolution != null)
            {
                _AnimationIndex++;
                if (_AnimationIndex >= _TargetSolution.PlayerPath.Directions.Count)
                {
                    _TargetSolution = null;
                    _IsAnimatingSolution = false;
                }
                AnimateSolution();
            }

            if (_DelayedKeys.Count < 3 && _DelayedKeys.Count > 0)
            {
                ProcessPressedKey(_DelayedKeys.Dequeue());
            }
            _DelayedKeys.Clear();
        }

        private void ShowLevelCompletedPopup()
        {
            int movesCount = stackHistoryIncidents.Count(h => h.HistoryObject.GetType() == typeof(Player));
            int pushesCount = stackHistoryIncidents.Count(h => h.HistoryObject.GetType() == typeof(Box));

            ShowModalPopup("Level Solved", "Congratulations, Level Solved in " + movesCount.ToString() + " moves and " + pushesCount.ToString() + " pushes." + Environment.NewLine + Environment.NewLine + "Click X to continue playing the next level.");

            //Load Next level
            _LevelIndex++;
            if (_LevelIndex < _LstLevelsPaths.Count)
            {
                ResetCurrentLevel();
                DownloadLevel(_LevelIndex);
            }
            else
            {
                ShowModalPopup("Game Completed", "Congratulations, You have finished the game." + Environment.NewLine + Environment.NewLine + "Thank you for playing sokoban.");
            }
        }

        void submitProgressOperation_Completed(object sender, EventArgs e)
        {

            activitySavingProgress.IsActive = false;
            ShowLevelCompletedPopup();
        }

        private void ResetCurrentLevel()
        {
            lstBoxes.Clear();
            lstFloor.Clear();
            lstTargets.Clear();
            lstWalls.Clear();
            mainCanvas.Children.Clear();
            stackHistoryIncidents.Clear();
            storyBoard.Stop();
            storyBoard.Children.Clear();

            _IsPlaying = false;
            _LevelTime = 0;

            tbMoves.Text = "Moves: 0";
            tbPushes.Text = "Pushes: 0";
            tbTime.Text = "Time: 0";
        }

        public IBlockPosition GetBlockByPosition(double left, double top)
        {
            Wall wallInstance = lstWalls.SingleOrDefault(b => (b.position.Left == left && b.position.Top == top));
            if (wallInstance != null) return wallInstance;

            Box boxInstance = lstBoxes.SingleOrDefault(b => (b.position.Left == left && b.position.Top == top));
            if (boxInstance != null) return boxInstance;

            Floor floorInstance = lstFloor.SingleOrDefault(b => (b.position.Left == left && b.position.Top == top));
            if (floorInstance != null) return floorInstance;

            Target targetInstance = lstTargets.SingleOrDefault(b => (b.position.Left == left && b.position.Top == top));
            if (targetInstance != null) return targetInstance;

            throw new Exception("Block type not recognized");
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double oldZoomFactor = _ZoomFactor;

            SetLevelDimensions();

            foreach (Control ctrl in mainCanvas.Children)
            {
                if (typeof(IBlockPosition).IsAssignableFrom(ctrl.GetType()))
                {
                    IBlockPosition block = (IBlockPosition)ctrl;
                    ctrl.SetValue(Canvas.LeftProperty, block.position.Left * _ZoomFactor);
                    ctrl.SetValue(Canvas.TopProperty, block.position.Top * _ZoomFactor);

                    ctrl.SetValue(UserControl.WidthProperty, _BlockSize * _ZoomFactor);

                    if (typeof(IBlockAnimation).IsAssignableFrom(ctrl.GetType()))
                    {
                        IBlockAnimation animationBlock = (IBlockAnimation)ctrl;
                        animationBlock.HorizontalTimeLine.To = animationBlock.position.Left * _ZoomFactor;
                        animationBlock.VerticalTimeLine.To = animationBlock.position.Top * _ZoomFactor;
                    }
                }
            }

        }

    }
}
