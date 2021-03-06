﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Win32;
using Model;

namespace ViewModel
{
    class ViewModel : INotifyPropertyChanged
    {
        #region Fileds

        ButtonList ContentAndcolors;
        QuestionsAndAnswerOptions questions = new QuestionsAndAnswerOptions();
        string[] contentButton;
        bool buttonIsEnable = false;
        private RelayCommand newQuestionAndStartTest;
        private RelayCommand checkForTheCorrectAnswer;
        private RelayCommand saveProfile;
        private RelayCommand exit;
        private RelayCommand openProfile;
        private RelayCommand indexСhange;
        private RelayCommand openQuestions;
        private string question;
        int[] correctAndMisteke = new int[2];
        int last_index_int;
        int start_index_int;
        string userName = "New User";
        int mistekeAnswerINT;
        int correctAnswerINT;
        #endregion


        #region properties
        public string UserName { get => userName; set { userName = value; OnPropertyChanged("UserName"); } }
        public int List_count { get => Questions.LenghtList; }
        public QuestionsAndAnswerOptions Questions { get => questions; set => questions = value; }

        public int[] CorrectAndMisteke { get => correctAndMisteke; set { correctAndMisteke = value; OnPropertyChanged("CorrectAndMisteke"); } }
        public int MistekeAnswerINT { get => mistekeAnswerINT; set { mistekeAnswerINT = value; OnPropertyChanged("MistekeAnswerINT"); } }
        public int CorrectAnswerINT { get => correctAnswerINT; set { correctAnswerINT = value; OnPropertyChanged("CorrectAnswerINT"); } }
        public bool ButtonIsEnable { get => buttonIsEnable; set { buttonIsEnable = value; OnPropertyChanged("ButtonIsEnable"); } }
        public string[] ContentButton { get => contentButton; set { contentButton = value; OnPropertyChanged("ContentButton"); } }
        public string Question { get => question; set { question = value; OnPropertyChanged("Question"); } }
        public bool SaveProfileOnExit { get; set; }
        public DateTime SaveProfileOnExitTime { get; set; }

        #region start and last index
        public int Start_index
        {
            get { return start_index_int; }
            set
            {
                if (start_index_int > -1 && value != -1)
                {
                    if (value < Last_index - 4)
                        start_index_int = value;
                    if (value >= Last_index - 4 && Last_index < List_count)
                    {
                        Last_index++;
                        start_index_int = value;
                    }
                }
                OnPropertyChanged("Start_index");
            }
        }
        public int Last_index
        {
            get { return last_index_int; }
            set
            {
                if (last_index_int < List_count + 1 && value <= List_count)
                {
                    if (value > Start_index + 4)
                        last_index_int = value;
                    if (value <= Start_index + 4 && Start_index > 0)
                    {
                        Start_index--;
                        last_index_int = value;
                    }
                }
                OnPropertyChanged("Last_index");
            }
        }

        #endregion

        #endregion


        #region Commands

        public RelayCommand CheckForTheCorrectAnswer
        {
            get
            {
                return checkForTheCorrectAnswer ??
                  (checkForTheCorrectAnswer = new RelayCommand(obj =>
                  {
                      if (!ContentAndcolors.ButtonStart.IsEnabled)
                      {
                          Button but = obj as Button;
                          if (but != null)
                          {
                              ContentAndcolors.AddButtonList(but);
                              MistekeAnswerINT = ContentAndcolors.MistekeAnswerINT;
                              CorrectAnswerINT = ContentAndcolors.CorrectAnswerINT;
                          }
                      }
                      else

                          Question = "Plis click NEXT";
                  }));
            }
        }
        public RelayCommand NewQuestionAndStartTest
        {
            get
            {
                return newQuestionAndStartTest ??
                  (newQuestionAndStartTest = new RelayCommand(obj =>
                  {
                      Button but = obj as Button;
                      if (but != null)
                      {
                          if (ContentAndcolors == null)
                          {
                              ContentAndcolors = new ButtonList(but);
                              Questions = new QuestionsAndAnswerOptions();
                              ContentAndcolors.Questions = Questions;
                              ButtonIsEnable = true;
                              MistekeAnswerINT = 0;
                              CorrectAnswerINT = 0;

                              Last_index = ContentAndcolors.Questions.LenghtList;
                              Start_index = 0;
                          }
                          ContentAndcolors.NewRandomQuestion(Start_index, Last_index);
                          Question = ContentAndcolors.Question;
                          ContentButton = ContentAndcolors.ContentButton;
                      }
                  }));
            }
        }
        public RelayCommand IndexChange
        {
            get
            {
                return indexСhange ??
                  (indexСhange = new RelayCommand(obj =>
                  {
                      RepeatButton but = obj as RepeatButton;
                      if (but != null)
                      {
                          if (but.Content.ToString() == "<")
                          {
                              if (but.Name == "plus_start_index")
                              {
                                  Start_index--;

                              }
                              else
                              {
                                  Last_index--;

                              }
                          }
                          else
                          {
                              if (but.Name == "minus_start_index")
                              {
                                  Start_index++;

                              }
                              else
                              {
                                  Last_index++;

                              }
                          }
                      }

                  }));
            }
        }

        #region Open and seve profile

        public RelayCommand SaveProfile
        {
            get
            {
                return saveProfile ??
                  (saveProfile = new RelayCommand(obj =>
                  {
                      string mydocu = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\tv\";
                      if (!Directory.Exists(mydocu))
                          Directory.CreateDirectory(mydocu);
                      SaveFileDialog sfd = new SaveFileDialog
                      {
                          FileName = UserName,
                          Filter = "vbdata files(*.vbdata) | *.vbdata",
                          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\tv\"
                      };

                      if (sfd.ShowDialog() == true)
                      {
                          SaveProfileOnExitTime = DateTime.Now;
                          SaveProfileOnExit = true;
                          mydocu = $"{sfd.FileName}.vbdata";
                          StreamWriter sr = File.CreateText(mydocu);
                          sr.WriteLine($"{UserName}");
                          sr.WriteLine($"{CorrectAnswerINT}");
                          sr.WriteLine($"{MistekeAnswerINT}");
                          sr.Close();
                          MessageBox.Show($"Profile {UserName} save");
                      }

                  }));
            }
        }
        public RelayCommand Exit
        {
            get
            {
                return exit ??
                  (exit = new RelayCommand(obj =>
                  {
                      Application.Current.Shutdown();
                      //if ((DateTime.Now - SaveProfileOnExitTime).TotalSeconds < 15)
                      //{
                      //   Application.Current.Shutdown();
                      //}
                      //else
                      //{
                      //    DialogWindow dialog = new DialogWindow();
                      //    if (dialog.ShowDialog() == true)
                      //    {
                      //        // Пользователь разрешил действие. Продолжить1
                      //    }
                      //    else
                      //    {
                      //        // Пользователь отменил действие.
                      //    }
                      //}


                  }));
            }
        }
        public RelayCommand OpenProfile
        {
            get
            {
                return openProfile ?? (openProfile = new RelayCommand(obj =>
                {
                    string filePath;
                    OpenFileDialog ofd = new OpenFileDialog
                    {
                        Filter = "vbdata files(*.vbdata) | *.vbdata",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\tv\"
                    };
                    if (ofd.ShowDialog() == true)
                    {
                        filePath = ofd.FileName;
                        StreamReader sr = File.OpenText(filePath);
                        try
                        {
                            UserName = sr.ReadLine();
                            CorrectAndMisteke[0] = int.Parse(sr.ReadLine());
                            CorrectAndMisteke[1] = int.Parse(sr.ReadLine());

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Невозможно открыть файл");
                        }
                    }
                }));
            }
        }

        #endregion
        #region Open questions
        public RelayCommand OpenQuestions
        {
            get
            {
                return openQuestions ?? (openQuestions = new RelayCommand(obj =>
                {

                        OpenFileDialog ofd = new OpenFileDialog
                        {
                            Filter = "Text Files(*.txt)|*.txt|All Files (*.*)|*.*",
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\tv\",
                        };

                    if (ofd.ShowDialog() == true)
                    {
                        Questions.NewQuestions(ofd.FileName);
                        Last_index = Questions.LenghtList;
                        if (ContentAndcolors != null)
                        {
                            ContentAndcolors.ButtonStart.IsEnabled = true;
                            Question = "Plis click NEXT";
                        }
                        CorrectAndMisteke[0] = 0;
                        CorrectAndMisteke[1] = 0;
                    }
                }));
            }
        }
        #endregion

        #endregion


        #region PropertyChangedEventHandler


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion


    }
}

