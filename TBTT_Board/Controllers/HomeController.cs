using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TBTT_Board.Models;
using TBTT_Data.Interface;
using TBTT_Data.Entities;
using AutoMapper;
using System.Timers;
using TBTT_Board.Helper;
using TBTT_Logging;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using TBTT_Board.ActionFilter;
using Microsoft.Extensions.Configuration;

namespace TBTT_Board.Controllers
{
    [TypeFilter(typeof(CustomExceptionFilter))]
    [ShortCircuitingResourceFilter]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IAvailableRepository _availableService;
        private IWaitingRepository _waitingService;
        private ICourtRepository _courtService;
        private IGameRepository _gameService;
        private IMemberRepository _memberService;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;

        public const string CUSTOM_ERROR = "CUSTOM_ERROR";
        public const string GENERIC_ERROR = "Errors Detected";
        public const string SUCCESS_MESSAGE = "Successfully Saved";
        public const string DUPLICATE_MESSAGE = "Duplicate Name";
        public const string INVALID_MEMBERSHIPID = "Invalid MembershipID";
        public const string ACTIONNOTPERFORMED = "Action Not Performed";
        public const string LIMIT_MESSAGE = "Exceeded Count";
        public const string NOTFOUND_MESSAGE = "Record not found";

        public HomeController(ILogger<HomeController> logger, IAvailableRepository availableService, IWaitingRepository waitingService, IGameRepository gameService, ICourtRepository courtService, IMapper mapper, IConfiguration configuration, IMemberRepository memberService)
        {
            _logger = logger;
            _courtService = courtService;
            _availableService = availableService;
            _waitingService = waitingService;
            _gameService = gameService;
            _memberService = memberService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //var efd = Helpers.GetFlogDetail("Index", null);
            //Logger.WriteDiagnostic(efd);

            //var databaseConfig = new TBTTDatabaseSettings();
            //_configuration.GetSection("TBTTDatabaseSettings").Bind(databaseConfig);
            //var databaseConnection = databaseConfig.ConnectionString;

            AvailableViewModel availableVM = new AvailableViewModel();
            Available availableDTO = new Available();
            DateTime availableDate = DateTime.Now.Date;
                       

            availableDTO = _availableService.GetAvailableList(string.Empty);
            availableVM = _mapper.Map<AvailableViewModel>(availableDTO);

            CourtViewModel courtVM = new CourtViewModel();
            Court courtDTO = new Court();

            courtDTO = _courtService.GetCourtList();
            courtVM = _mapper.Map<CourtViewModel>(courtDTO);

            if (courtVM != null)
            {
                foreach (var item in courtVM.CourtList)
                {
                    if (item.Visibility.ToLower() == "false")
                    {
                        deleteFromGameBoardHelperOnVisibility(item.CourtName);
                    }
                }
                availableVM.CourtList = courtVM.CourtList;
            }

            return View(availableVM);
        }


      
        [HttpGet]
        public ActionResult GetAvailableList(string AvailableName)
        {
            //var efd = Helpers.GetFlogDetail("GetAvailableList", null);
            //Logger.WriteDiagnostic(efd);

            AvailableViewModel availableVM = new AvailableViewModel();
            Available availableDTO = new Available();

            availableDTO = _availableService.GetAvailableList(AvailableName);
            availableVM = _mapper.Map<AvailableViewModel>(availableDTO);

            return PartialView("_availableList", availableVM.AvailableList);
        }

        [HttpPost]
        public ActionResult AddToAvailableList([FromForm]string formData)
        {
            //var efd = Helpers.GetFlogDetail("AddToAvailableList", null);
            //Logger.WriteDiagnostic(efd);

            DateTime availableDate = DateTime.Now.Date;

            Available availableDTO = new Available();
            Member memberDTO = new Member();
            MemberViewModel memberVM = new MemberViewModel();
            Game game = new Game();
            Waiting waiting = new Waiting();
            int intReturnCode = -2;
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            bool isMember = false;
            bool isGuest = false;
            string memberName = string.Empty;
            string memberAliasName = string.Empty;
            string memberID = string.Empty;
            string memberGender = string.Empty;
            string memberScore = string.Empty;
            Int32 intMemberScore = 1800;
            string memberDOB = string.Empty;
            string memberMembershipType = "N";
            string memberBillingType = "D";
            Int32 intmemberDOB = 1900;
            string[] lines = null;
            string firstLine = string.Empty;
            string secondLine = string.Empty;
            string thirdLine = string.Empty;
            string dataString = string.Empty;
            string memberIDPattern = @"\%?[ ]*[b]\d+[ ]*[\?]?"; //%B00050?
            RegexOptions options = RegexOptions.Multiline;
            options = RegexOptions.IgnoreCase;
            int intCount = 0;
            bool isSuccess = true;
            int intData = 0;
            string dataPart = string.Empty;

            try
            {
                if ((formData != null) && (formData.Length>0))
                {
                    lines = formData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                }

                if ((lines != null) && (lines.Length > 0))
                {
                    intCount = 0;
                    for (int i=0; i <= lines.Length-1; i++)
                    {
                        dataString = lines[i];
                        if ((dataString != null) && (dataString.Length >0))
                        {
                            intCount += 1;

                            switch (intCount)
                            {
                                case 1:
                                    firstLine = dataString.Trim().ToUpper();
                                    break;
                                case 2:
                                    secondLine = dataString.Trim();
                                    break;
                                default:
                                    thirdLine = dataString.Trim();
                                    break;
                            }
                        }
                    }
                }

                if ((firstLine !=null) && (firstLine.Length > 0))
                {
                    Match memberIDMatch = Regex.Match(firstLine, memberIDPattern, options);

                    if ((memberIDMatch != null) && (memberIDMatch.Length >0))
                    {
                        dataString = memberIDMatch.ToString().Replace("?", string.Empty).Replace("%", string.Empty);
                        memberID = dataString.Trim();

                        if ((memberID.IndexOf('9') >= 0)  && (memberID.IndexOf('9') <= 1) && (memberID.Length==6))
                        {
                            isGuest = true;
                            memberMembershipType = "G";
                            memberBillingType = "D";
                            memberName = "Guest";
                        }
                        if ((memberID.ToUpper().IndexOf('B') >= 0)  && (memberID.ToUpper().IndexOf('B') <= 1) && (memberID.Length == 6))
                        {
                            isMember = true;
                            memberMembershipType = "M";
                            memberBillingType = "M";
                        }                        
                    }
                }

                if ((secondLine != null) && (secondLine.Length > 0))
                {
                    if (secondLine.Length > 10)
                    {
                        memberName = secondLine.Substring(0,9).Trim();
                    }
                    else
                    {
                        memberName = secondLine.Trim();
                    }
                    
                }

                if ((thirdLine != null) && (thirdLine.Length > 0))
                {
                    dataString = thirdLine.Trim();
                    lines = null;
                    lines = thirdLine.Split("-");
                    if ((lines != null) && (lines.Length > 0))
                    {
                        intCount = 0;
                    }
                    for (int i = 0; i <= lines.Length - 1; i++)
                    {
                        dataString = lines[i];
                        if ((dataString != null) && (dataString.Length > 0))
                        {
                            intCount += 1;

                            switch (intCount)
                            {
                                case 1:
                                    memberGender = dataString.Trim();                                    
                                    break;
                                case 2:
                                    memberDOB = dataString.Trim();
                                    break;
                                default:
                                    memberScore = dataString.Trim();
                                    break;
                            }
                        }
                    }

                }

                if (memberGender.Length > 0)
                {
                    if ((memberGender == "Y") || (memberGender == "F"))
                    {
                        memberGender = memberGender.Trim().ToUpper();
                    }
                    else
                    {
                        memberGender = string.Empty;
                    }
                }

                if (memberDOB.Length > 0)
                {
                    intData = 0;
                    isSuccess = Int32.TryParse(memberDOB, out intData);

                    if (intData > 0)
                    {
                        intmemberDOB = intData;
                    }
                    else
                    {
                        intmemberDOB = 1900;
                    }
                }

                if (memberScore.Length > 0)
                {
                    intData = 0;
                    isSuccess = Int32.TryParse(memberScore, out intData);

                    if (intData > 0)
                    {
                        intMemberScore = intData;
                    }
                    else
                    {
                        intMemberScore = 0;
                    }
                }


                if (memberID == string.Empty)
                {
                    isMember = false;
                    isGuest = false;
                    if ((formData!=null) && (formData.Length > 10))
                    {
                        memberName = formData.Substring(0, 9).Trim();
                    }
                    else
                    {
                        memberName = formData.Trim();
                    }                    
                }

                if ((memberName != null) && (memberName.Length > 0))
                {
                    memberName = myTI.ToTitleCase(memberName.ToLower().Trim());
                    memberAliasName = memberName;
                }



                memberDTO = _memberService.GetMember(memberID);
                memberVM = _mapper.Map<MemberViewModel>(memberDTO);

                if ((memberVM != null) && (memberVM.MemberID != null))
                {
                    memberID = memberVM.MemberID;
                    memberName = memberVM.MemberName;
                    memberAliasName = memberVM.AliasName;
                }
                else
                {
                    if ((isMember) || (isGuest))
                    {
                        if ((memberID != null) && (memberID.Length > 2))
                        {
                            intData = 0;
                            dataPart = string.Empty;
                            lines = null;
                            lines = memberName.Split(' ');
                            string firstPart = string.Empty;

                            if ((lines != null) && (lines.Length > 0))
                            {
                                firstPart = lines[0];
                            }

                            if (isGuest)
                            {
                                dataPart = memberID.Substring(2, memberID.Length - 2);
                                firstPart = "Guest";
                            }
                            else
                            {
                                dataPart = memberID.Trim().Substring(1, memberID.Length - 1);
                            }
                            
                            isSuccess = Int32.TryParse(dataPart, out intData);
                            memberAliasName = $"{intData}-{firstPart}";

                            if (firstPart == string.Empty)
                            {
                                return Json(new { success = false, responseText = INVALID_MEMBERSHIPID });
                            }
                        }

                        if (memberAliasName == string.Empty)
                        {
                            return Json(new { success = false, responseText = INVALID_MEMBERSHIPID });
                        }

                        memberDTO = new Member();
                        memberDTO.MemberID = memberID;
                        memberDTO.MemberName = memberName;
                        memberDTO.AliasName = memberAliasName;
                        memberDTO.DateOfBirth = new DateTime(intmemberDOB, 1,1); 
                        memberDTO.Gender = memberGender;
                        memberDTO.Score = intMemberScore;
                        memberDTO.MembershipType = memberMembershipType;
                        memberDTO.BillingType = memberBillingType;
                        memberDTO.Status = 1;
                        _memberService.AddMember(memberDTO);
                    }
                }

                if (memberID == string.Empty)
                {
                    memberID = Guid.NewGuid().ToString();
                }

                if (memberAliasName == string.Empty)
                {
                    return Json(new { success = false, responseText = INVALID_MEMBERSHIPID });
                }

                availableDTO.AvailableListID = 1;
                availableDTO.MembershipID = memberID;
                availableDTO.MemberName = memberAliasName;
                availableDTO.MembershipType = memberMembershipType;
                availableDTO.NodeState = true;
                availableDTO.BoardStartDate = availableDate;
                availableDTO.UpdatedDate = DateTime.Now;

          

                game = _gameService.GetGameBoardByMemberName(memberAliasName);
           
                if (game == null)
                {
                    waiting = _waitingService.GetWaitingListByMemberName(memberAliasName);

                    if (waiting == null)
                    {
                        //var efd1 = Helpers.GetFlogDetail("AddToAvailableList" + availableDTO.MembershipID, null);
                        //Logger.WriteDiagnostic(efd1);

                        intReturnCode = _availableService.AddToAvailableList(availableDTO);

                        //var efd2 = Helpers.GetFlogDetail("AddToAvailableList - Return Code" + intReturnCode.ToString(), null);
                        //Logger.WriteDiagnostic(efd2);

                    }

                }

               

                if (intReturnCode == -2)
                {
                    return Json(new { success = false, responseText = DUPLICATE_MESSAGE });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = GENERIC_ERROR });            }

            return Json(new { success = false, responseText = GENERIC_ERROR });

        }

        [HttpPost]
        public ActionResult AddToWaitingListFromSearch([FromForm] string formData)
        {
            //var efd = Helpers.GetFlogDetail("AddToAvailableList", null);
            //Logger.WriteDiagnostic(efd);

            DateTime availableDate = DateTime.Now.Date;

            Waiting waitingDTO = new Waiting();
            Member memberDTO = new Member();
            MemberViewModel memberVM = new MemberViewModel();
            Game game = new Game();
            Waiting waiting = new Waiting();
            int intReturnCode = -2;
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            bool isMember = false;
            bool isGuest = false;
            string memberName = string.Empty;
            string memberAliasName = string.Empty;
            string memberID = string.Empty;
            string memberGender = string.Empty;
            string memberScore = string.Empty;
            Int32 intMemberScore = 1800;
            string memberDOB = string.Empty;
            string memberMembershipType = "N";
            string memberBillingType = "D";
            Int32 intmemberDOB = 1900;
            string[] lines = null;
            string firstLine = string.Empty;
            string secondLine = string.Empty;
            string thirdLine = string.Empty;
            string dataString = string.Empty;
            string memberIDPattern = @"\%?[ ]*[b]\d+[ ]*[\?]?"; //%B00050?
            RegexOptions options = RegexOptions.Multiline;
            options = RegexOptions.IgnoreCase;
            int intCount = 0;
            bool isSuccess = true;
            int intData = 0;
            string dataPart = string.Empty;

            try
            {
                if ((formData != null) && (formData.Length > 0))
                {
                    lines = formData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                }

                if ((lines != null) && (lines.Length > 0))
                {
                    intCount = 0;
                    for (int i = 0; i <= lines.Length - 1; i++)
                    {
                        dataString = lines[i];
                        if ((dataString != null) && (dataString.Length > 0))
                        {
                            intCount += 1;

                            switch (intCount)
                            {
                                case 1:
                                    firstLine = dataString.Trim().ToUpper();
                                    break;
                                case 2:
                                    secondLine = dataString.Trim();
                                    break;
                                default:
                                    thirdLine = dataString.Trim();
                                    break;
                            }
                        }
                    }
                }

                if ((firstLine != null) && (firstLine.Length > 0))
                {
                    Match memberIDMatch = Regex.Match(firstLine, memberIDPattern, options);

                    if ((memberIDMatch != null) && (memberIDMatch.Length > 0))
                    {
                        dataString = memberIDMatch.ToString().Replace("?", string.Empty).Replace("%", string.Empty);
                        memberID = dataString.Trim();

                        if ((memberID.IndexOf('9') >= 0) && (memberID.IndexOf('9') <= 1) && (memberID.Length == 6))
                        {
                            isGuest = true;
                            memberMembershipType = "G";
                            memberBillingType = "D";
                            memberName = "Guest";
                        }
                        if ((memberID.ToUpper().IndexOf('B') >= 0) && (memberID.ToUpper().IndexOf('B') <= 1) && (memberID.Length == 6))
                        {
                            isMember = true;
                            memberMembershipType = "M";
                            memberBillingType = "M";
                        }
                    }
                }

                if ((secondLine != null) && (secondLine.Length > 0))
                {
                    if (secondLine.Length > 10)
                    {
                        memberName = secondLine.Substring(0, 9).Trim();
                    }
                    else
                    {
                        memberName = secondLine.Trim();
                    }

                }

                if ((thirdLine != null) && (thirdLine.Length > 0))
                {
                    dataString = thirdLine.Trim();
                    lines = null;
                    lines = thirdLine.Split("-");
                    if ((lines != null) && (lines.Length > 0))
                    {
                        intCount = 0;
                    }
                    for (int i = 0; i <= lines.Length - 1; i++)
                    {
                        dataString = lines[i];
                        if ((dataString != null) && (dataString.Length > 0))
                        {
                            intCount += 1;

                            switch (intCount)
                            {
                                case 1:
                                    memberGender = dataString.Trim();
                                    break;
                                case 2:
                                    memberDOB = dataString.Trim();
                                    break;
                                default:
                                    memberScore = dataString.Trim();
                                    break;
                            }
                        }
                    }

                }

                if (memberGender.Length > 0)
                {
                    if ((memberGender == "Y") || (memberGender == "F"))
                    {
                        memberGender = memberGender.Trim().ToUpper();
                    }
                    else
                    {
                        memberGender = string.Empty;
                    }
                }

                if (memberDOB.Length > 0)
                {
                    intData = 0;
                    isSuccess = Int32.TryParse(memberDOB, out intData);

                    if (intData > 0)
                    {
                        intmemberDOB = intData;
                    }
                    else
                    {
                        intmemberDOB = 1900;
                    }
                }

                if (memberScore.Length > 0)
                {
                    intData = 0;
                    isSuccess = Int32.TryParse(memberScore, out intData);

                    if (intData > 0)
                    {
                        intMemberScore = intData;
                    }
                    else
                    {
                        intMemberScore = 0;
                    }
                }


                if (memberID == string.Empty)
                {
                    isMember = false;
                    isGuest = false;
                    if ((formData != null) && (formData.Length > 10))
                    {
                        memberName = formData.Substring(0, 9).Trim();
                    }
                    else
                    {
                        memberName = formData.Trim();
                    }
                }

                if ((memberName != null) && (memberName.Length > 0))
                {
                    memberName = myTI.ToTitleCase(memberName.ToLower().Trim());
                    memberAliasName = memberName;
                }



                memberDTO = _memberService.GetMember(memberID);
                memberVM = _mapper.Map<MemberViewModel>(memberDTO);

                if ((memberVM != null) && (memberVM.MemberID != null))
                {
                    memberID = memberVM.MemberID;
                    memberName = memberVM.MemberName;
                    memberAliasName = memberVM.AliasName;
                }
                else
                {
                    if ((isMember) || (isGuest))
                    {
                        if ((memberID != null) && (memberID.Length > 2))
                        {
                            intData = 0;
                            dataPart = string.Empty;
                            lines = null;
                            lines = memberName.Split(' ');
                            string firstPart = string.Empty;

                            if ((lines != null) && (lines.Length > 0))
                            {
                                firstPart = lines[0];
                            }

                            if (isGuest)
                            {
                                dataPart = memberID.Substring(2, memberID.Length - 2);
                                firstPart = "Guest";
                            }
                            else
                            {
                                dataPart = memberID.Trim().Substring(1, memberID.Length - 1);
                            }

                            isSuccess = Int32.TryParse(dataPart, out intData);
                            memberAliasName = $"{intData}-{firstPart}";

                            if (firstPart == string.Empty)
                            {
                                return Json(new { success = false, responseText = INVALID_MEMBERSHIPID });
                            }
                        }

                        if (memberAliasName == string.Empty)
                        {
                            return Json(new { success = false, responseText = INVALID_MEMBERSHIPID });
                        }

                        memberDTO = new Member();
                        memberDTO.MemberID = memberID;
                        memberDTO.MemberName = memberName;
                        memberDTO.AliasName = memberAliasName;
                        memberDTO.DateOfBirth = new DateTime(intmemberDOB, 1, 1);
                        memberDTO.Gender = memberGender;
                        memberDTO.Score = intMemberScore;
                        memberDTO.MembershipType = memberMembershipType;
                        memberDTO.BillingType = memberBillingType;
                        memberDTO.Status = 1;
                        _memberService.AddMember(memberDTO);
                    }
                }

                if (memberID == string.Empty)
                {
                    memberID = Guid.NewGuid().ToString();
                }

                if (memberAliasName == string.Empty)
                {
                    return Json(new { success = false, responseText = INVALID_MEMBERSHIPID });
                }

                waitingDTO.WaitingListID = 1;
                waitingDTO.MembershipID = memberID;
                waitingDTO.MemberName = memberAliasName;
                waitingDTO.MembershipType = memberMembershipType;
                waitingDTO.NodeState = true;
                waitingDTO.BoardStartDate = availableDate;
                waitingDTO.UpdatedDate = DateTime.Now;

                game = _gameService.GetGameBoardByMemberName(memberAliasName);

                if (game == null)
                {
                    intReturnCode = _waitingService.AddToWaitingList(waitingDTO);

                    waiting = _waitingService.GetWaitingListByMemberID(memberID);

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = GENERIC_ERROR });
            }

            return Json(new { success = false, responseText = GENERIC_ERROR });

        }

        [HttpGet]
        public ActionResult GetWaitingList(string PlayingName)
        {
            //var efd = Helpers.GetFlogDetail("GetWaitingList", null);
            //Logger.WriteDiagnostic(efd);

            Waiting waitingDTO = new Waiting();
            WaitingViewModel waitingVM = new WaitingViewModel();
            string waitingDate = DateTime.Now.ToShortDateString();

            waitingDTO = _waitingService.GetWaitingList(PlayingName);
            waitingVM = _mapper.Map<WaitingViewModel>(waitingDTO);

            return PartialView("_waitingList", waitingVM.WaitingList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToWaitingList([FromForm] string MembershipID, string MemberName, string MembershipType)
        {
            //var efd = Helpers.GetFlogDetail("AddToWaitingList", null);
            //Logger.WriteDiagnostic(efd);

            int intReturnValue = 0;
            ActionResult actionResult;
            Waiting waitingDTO = new Waiting();
            string availableDate = DateTime.Now.ToShortDateString();
            Available avail = new Available();

            try
            {
                waitingDTO.MembershipID = MembershipID;
                waitingDTO.MemberName = MemberName.Trim();
                waitingDTO.MembershipType = MembershipType.Trim();
                waitingDTO.NodeState = true;

                avail = _availableService.GetAvailableListByMemberID(MembershipID);

                if (avail == null)
                {
                    return Json(new { success = false, responseText = NOTFOUND_MESSAGE });
                }


                intReturnValue = _waitingService.AddToWaitingList(waitingDTO);

                if (intReturnValue == 1) {
                    intReturnValue = _availableService.DeleteFromAvailableListByMemberID(MembershipID);
                }

                if (intReturnValue == 1)
                {
                    return Json(new { success = true, responseText = SUCCESS_MESSAGE });
                }               
            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = GENERIC_ERROR });
            }

            return Json(new { success = false, responseText = GENERIC_ERROR });

        }

        [HttpPost]
        public ActionResult WaitingListUpdateStatus([FromForm] string MembershipID, bool NodeStatus)
        {
            //var efd = Helpers.GetFlogDetail("WaitingListUpdateStatus", null);
            //Logger.WriteDiagnostic(efd);

            int intReturnValue = 0;
            Waiting waitingDTO = new Waiting();
           
            try
            {
                waitingDTO.MembershipID = MembershipID;
                waitingDTO.NodeState = NodeStatus;

                intReturnValue = _waitingService.WaitingListUpdateStatus(waitingDTO);

 
                if (intReturnValue == 1)
                {
                    return Json(new { success = true, responseText = SUCCESS_MESSAGE });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = GENERIC_ERROR });
            }

            return Json(new { success = false, responseText = GENERIC_ERROR });

        }

        [HttpGet]
         public ActionResult GetGameBoard(string courtName)
        {
            //var efd = Helpers.GetFlogDetail("GetGameBoard", null);
            //Logger.WriteDiagnostic(efd);
            Game gamegDTO = new Game();
            GameViewModel gameVM = new GameViewModel();
            DateTime minGameStartDate;
            int intReturnValue = 1;
            //string elapsedTimeFormat = string.Empty;

            double elapsedTime;
            DateTime today = DateTime.Now;
            DateTime minGameStartDateEST = today;
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            int nodeCount = 0;
            double totalSeconds;
            double totalMinutes = 0;
            TimeSpan totalDifference;
            intReturnValue = deleteFromGameBoardHelper(courtName);
            gamegDTO = _gameService.GetGameBoard(courtName);
            gameVM = _mapper.Map<GameViewModel>(gamegDTO);
            string formatedElapsedTime = string.Empty;

            if ((gameVM != null) && (gameVM.GameList.Count() > 0))
             {
                minGameStartDate = gameVM.GameList.Min(x => x.GameStartDate);
               // elapsedTime =Math.Round((today - minGameStartDate).TotalMinutes,2, MidpointRounding.AwayFromZero);

                totalDifference = today.Subtract(minGameStartDate);
                totalSeconds = totalDifference.Seconds;
                totalMinutes = totalDifference.Minutes;

                formatedElapsedTime = $"{totalMinutes.ToString().PadLeft(2,'0')}:{totalSeconds.ToString().PadLeft(2, '0')}";

                nodeCount = gameVM.GameList.Count();

                foreach (var item in gameVM.GameList)
                {
                    item.MinGameStartDate = minGameStartDateEST;
                    item.GameStopWatchstartMin = formatedElapsedTime; //elapsedTime
                    item.NodeCounter = nodeCount.ToString();
                }
             }
            

            return PartialView("_GameList", gameVM.GameList);
        }

        [HttpPost]
        public ActionResult AddToGameBoard([FromForm] string MembershipID, string MemberName, string CourtName, string OrderID, string MembershipType)
        {
            //var efd = Helpers.GetFlogDetail("AddToGameBoard", null);
            //Logger.WriteDiagnostic(efd);
            int intReturnValue = 0;
            ActionResult actionResult;
            Game gameDTO = new Game();
            Waiting waiting = new Waiting();
            int intOrderID=0;

            Court courtDTO = new Court();
            int intMemberCount = 4;
            bool isDoubles = true;
            bool isSuccess = true;

            try
            {

                int.TryParse(OrderID, out intOrderID);
                gameDTO.MembershipID = MembershipID;
                gameDTO.MemberName = MemberName.Trim();
                gameDTO.CourtName = CourtName;
                gameDTO.OrderID = intOrderID; 
                gameDTO.MembershipType = MembershipType;

                waiting = _waitingService.GetWaitingListByMemberID(MembershipID);

                if (waiting == null)
                {
                    return Json(new { success = false, responseText = NOTFOUND_MESSAGE });
                }

                courtDTO = _courtService.GetCourtByName(CourtName);

                if (courtDTO != null)
                {
                    if (courtDTO.IsDoubles != null)
                    {
                        isSuccess = bool.TryParse(courtDTO.IsDoubles, out isDoubles);
                        if (isDoubles)
                        {
                            intMemberCount = 4;
                        }
                        else
                        {
                            intMemberCount = 2;
                        }

                    }
                }


                intReturnValue = _gameService.AddToGameBoard(gameDTO, intMemberCount);

                if (intReturnValue == 1)
                {
                    intReturnValue = _waitingService.DeleteFromWaitingListByMemberID(MembershipID);
                }

                if (intReturnValue == 1)
                {
                    return Json(new { success = true, responseText = SUCCESS_MESSAGE });
                }

                if (intReturnValue == -2)
                {
                    return Json(new { success = false, responseText = LIMIT_MESSAGE });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = GENERIC_ERROR });
            }

            return Json(new { success = false, responseText = GENERIC_ERROR });

        }

        private int deleteFromGameBoardHelper(string CourtName)
        {
            //var efd = Helpers.GetFlogDetail("deleteFromGameBoardHelper", null);
            //Logger.WriteDiagnostic(efd);

            int intReturnValue = 1;

            bool isValidToDelete = true;
            Game gamegDTO = new Game();
            GameViewModel gameVM = new GameViewModel();

            bool isDoubles = true;
            bool isSuccess = true;
            Court courtDTO = new Court();
            int intMemberCount = 4;


            try
            {
                isValidToDelete = _gameService.IsValidToDeleteFromGameBoard(CourtName);

                if (isValidToDelete)
                {
                    courtDTO = _courtService.GetCourtByName(CourtName);

                    if (courtDTO != null)
                    {
                        if (courtDTO.IsDoubles != null)
                        {
                            isSuccess = bool.TryParse(courtDTO.IsDoubles, out isDoubles);
                            if (isDoubles)
                            {
                                intMemberCount = 4;
                            }
                            else
                            {
                                intMemberCount = 2;
                            }

                        }
                    }

                    gamegDTO = _gameService.GetGameBoardByLimitedNumber(CourtName, intMemberCount);

                    if ((gamegDTO != null) && (gamegDTO.GameList.Count() > 0))
                    {
                        
                        intReturnValue = _waitingService.AddToWaitingListFromGameBoard(gamegDTO);

                        if (intReturnValue == 1)
                        {
                            intReturnValue = _gameService.DeleteFromGameBoard(CourtName, intMemberCount);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                intReturnValue = -1;
            }


            return intReturnValue;
        }

        private int deleteFromGameBoardHelperOnVisibility(string CourtName)
        {
            //var efd = Helpers.GetFlogDetail("deleteFromGameBoardHelper", null);
            //Logger.WriteDiagnostic(efd);

            int intReturnValue = 1;

            bool isValidToDelete = true;
            Game gamegDTO = new Game();
            GameViewModel gameVM = new GameViewModel();

            try
            {
                    gamegDTO = _gameService.GetGameBoardOrderByOrderID(CourtName);

                    if ((gamegDTO != null) && (gamegDTO.GameList.Count() > 0))
                    {
                        intReturnValue = _waitingService.AddToWaitingListFromGameBoard(gamegDTO);

                        if (intReturnValue == 1)
                        {
                            intReturnValue = _gameService.DeleteFromGameBoardOnVisibility(CourtName);
                        }
                    }

            }
            catch (Exception ex)
            {
                intReturnValue = -1;
            }


            return intReturnValue;
        }

        [HttpPost]
        public ActionResult DeleteFromGameBoard([FromForm] string CourtName)
        {
            //var efd = Helpers.GetFlogDetail("DeleteFromGameBoard", null);
            //Logger.WriteDiagnostic(efd);
            int intReturnValue = 0;
  

            try
            {

                intReturnValue = deleteFromGameBoardHelper(CourtName);



                if (intReturnValue == 1)
                {
                    return Json(new { success = true, responseText = SUCCESS_MESSAGE });
                }
                else
                {
                    return Json(new { success = true, responseText = ACTIONNOTPERFORMED });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = GENERIC_ERROR });
            }

            return Json(new { success = false, responseText = GENERIC_ERROR });

        }

        [HttpPost]
        public ActionResult DeleteFromGameBoardByMembershipID([FromForm] string CourtName, string MemberName)
        {
            int intReturnValue = 0;

            try
            {
                intReturnValue = DeleteFromGameBoardByMembershipIDHelper(CourtName, MemberName);

                if (intReturnValue == 1)
                {
                    return Json(new { success = true, responseText = SUCCESS_MESSAGE });
                }
                else
                {
                    return Json(new { success = true, responseText = ACTIONNOTPERFORMED });
                }

            }
            catch (Exception)
            {

                return Json(new { success = false, responseText = GENERIC_ERROR });
            }

            return Json(new { success = false, responseText = GENERIC_ERROR });
        }

            private int DeleteFromGameBoardByMembershipIDHelper(string CourtName, string MemberName)
        {
            int intReturnValue = 1;
            Game gamegDTO = new Game();


            try
            {
                if (MemberName != null)
                {
                    MemberName = MemberName.Trim();
                }
                gamegDTO = _gameService.GetGameBoardByCourtAndMemberName(CourtName, MemberName);

                if (gamegDTO != null)
                {

                    intReturnValue = _gameService.DeleteFromGameBoardByMemberName(CourtName, MemberName);                   

                    if (intReturnValue == 1)
                    {
                        intReturnValue = _waitingService.AddToWaitingListFromGameBoard(gamegDTO);
                    }
                }
            }
            catch (Exception ex)
            {

                intReturnValue = -1;
            }

            return intReturnValue;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public ActionResult GetTBTTHelp()
        {
            string filePath = Convert.ToString(_configuration.GetSection("TBTT_Help").Value);
            Response.Headers.Add("Content-Disposition", "inline; filename=TBTT_Board_Help.pdf");
            return File(filePath, "application/pdf");
        }

    }
}
