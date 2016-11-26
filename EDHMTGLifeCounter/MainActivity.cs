using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Views;
using Android.Views.InputMethods;
using Android.Content;

namespace EDHMTGLifeCounter
{
    [Activity(Label = "EDHMTGLifeCounter", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ViewModel.LifeCounter mainActivityModel = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LifeCounter);

            EDHMTGDataAccess.SqliteConnection conn = new EDHMTGDataAccess.SqliteConnection();
            conn.SQLiteConnection();
            conn.CreateTables();

            var model = conn.GetLatestEntry();
            if (model == null || model.Life == 0)
            {
                ViewModel.LifeCounter lifeModel = new ViewModel.LifeCounter();
                lifeModel.CommanderDamageCounter = 0;
                lifeModel.PoisonDamageCounter = 0;
                lifeModel.Life = 40;
                var returnModel = conn.AddLifeCounter(Translate(lifeModel));
                mainActivityModel = Translate(returnModel);
            }
            else
            {
                mainActivityModel = Translate(model);
            }

            ActionBar.Tab tab = ActionBar.NewTab();
            tab.SetText("Life");

            tab.TabSelected += (sender, args) =>
            {
                conn.UpdateLifeCounter(Translate(mainActivityModel));
                SetContentView(Resource.Layout.LifeCounter);
                model = conn.GetLatestEntry();
                SetupLifeMainView(Translate(model));
            };
            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            tab.SetText("Poison");

            tab.TabSelected += (sender, args) =>
            {
                conn.UpdateLifeCounter(Translate(mainActivityModel));
                SetContentView(Resource.Layout.layPoison);
                model = conn.GetLatestEntry();
                SetupLifeMainView(Translate(model));
            };
            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();

            tab.SetText("Commander");

            tab.TabSelected += (sender, args) =>
            {
                conn.UpdateLifeCounter(Translate(mainActivityModel));
                SetContentView(Resource.Layout.layCommander);
                model = conn.GetLatestEntry();
                SetupLifeMainView(Translate(model));
            };
            ActionBar.AddTab(tab);


            ActionBar.Title = "EDH MTG Life Counter";
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_reset:
                    ResetAll();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void ResetAll()
        {
            Dialog dia = new Dialog(this);
            dia = new Dialog(this);
            dia.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dia.SetContentView(Resource.Layout.layAreYouSure);

            Button cancelButton = dia.FindViewById<Button>(Resource.Id.btnCancelDiag);
            cancelButton.Click += delegate { dia.Dismiss(); };

            Button okayButton = dia.FindViewById<Button>(Resource.Id.btnOkayDiag);
            okayButton.Click += delegate
            {
                TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);

                mainActivityModel.Life = 40;
                mainActivityModel.PoisonDamageCounter = 0;
                mainActivityModel.CommanderDamageCounter = 0;
                if (ActionBar.SelectedTab.Position == 0)
                {
                    txtHelth.Text = mainActivityModel.Life.ToString();
                }
                else if (ActionBar.SelectedTab.Position == 1)
                {
                    txtHelth.Text = mainActivityModel.PoisonDamageCounter.ToString();
                }
                else if (ActionBar.SelectedTab.Position == 2)
                {
                    txtHelth.Text = mainActivityModel.CommanderDamageCounter.ToString();
                }

                dia.Dismiss();
            };
            dia.Show();
        }

        private void SetupLifeMainView(ViewModel.LifeCounter ViewModel)
        {
            Button addFiveButton = FindViewById<Button>(Resource.Id.btnAddAmount);
            Button addOneButton = FindViewById<Button>(Resource.Id.btnAddOne);
            Button subtractOneButton = FindViewById<Button>(Resource.Id.btnSubtractOne);
            TextView counterText = FindViewById<TextView>(Resource.Id.txtHealth);
            if (ActionBar.SelectedTab.Position == 0)
            {
                counterText.Text = ViewModel.Life.ToString();
            }
            else if (ActionBar.SelectedTab.Position == 1)
            {
                counterText.Text = ViewModel.PoisonDamageCounter.ToString();

            }
            else if (ActionBar.SelectedTab.Position == 2)
            {
                counterText.Text = ViewModel.CommanderDamageCounter.ToString();

            }

            addFiveButton.Click += delegate { AddFiveHealth(); };
            addOneButton.Click += delegate { AddOneHealth(mainActivityModel); };
            subtractOneButton.Click += delegate { SubtractOneHealth(mainActivityModel); };

        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        private void AddFiveHealth()
        {
            var selectedTab = ActionBar.SelectedTab;

            Dialog dia = new Dialog(this);
            dia = new Dialog(this);
            dia.SetTitle("Enter amount (Ex: -5 or 5): ");
            dia.SetContentView(Resource.Layout.layAmount);
            Button cancelButton = dia.FindViewById<Button>(Resource.Id.btnCancelDialog);
            Button addAmountButton = dia.FindViewById<Button>(Resource.Id.btnAddAmountDialog);
            EditText amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            cancelButton.Click += delegate
            {
                HideKeyboard(amount, imm);
                dia.Dismiss();
            };
            if (selectedTab.Position == 0)
            {
                addAmountButton.Click += delegate
                {

                    amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
                    TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                    mainActivityModel.Life = Convert.ToInt16(amount.Text) + Convert.ToInt32(txtHelth.Text);
                    txtHelth.Text = (mainActivityModel.Life).ToString();
                    HideKeyboard(amount, imm);
                    dia.Dismiss();
                };
                dia.Show();
                ShowKeyboard(amount, imm);
                dia.DismissEvent += delegate
                {
                    HideKeyboard(amount, imm);
                };
            }
            else if (selectedTab.Position == 1)
            {
                addAmountButton.Click += delegate
                {
                    amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
                    TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                    mainActivityModel.PoisonDamageCounter = Convert.ToInt16(amount.Text) + Convert.ToInt32(txtHelth.Text);
                    txtHelth.Text = (mainActivityModel.PoisonDamageCounter).ToString();
                    HideKeyboard(amount, imm);
                    dia.Dismiss();
                };
                dia.Show();
                ShowKeyboard(amount, imm);
                dia.DismissEvent += delegate
                {
                    HideKeyboard(amount, imm);
                };
            }
            else
            {
                addAmountButton.Click += delegate
                {
                    amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
                    TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                    mainActivityModel.CommanderDamageCounter = Convert.ToInt16(amount.Text) + Convert.ToInt32(txtHelth.Text);
                    txtHelth.Text = (mainActivityModel.CommanderDamageCounter).ToString();
                    HideKeyboard(amount, imm);
                    dia.Dismiss();
                };
                dia.Show();
                ShowKeyboard(amount, imm);
                dia.DismissEvent += delegate
                {
                    HideKeyboard(amount, imm);
                };
            }
        }

        public static void ShowKeyboard(View pView, InputMethodManager imm)
        {
            pView.RequestFocus();

            imm.ShowSoftInput(pView, ShowFlags.Forced);
            imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        public static void HideKeyboard(View pView, InputMethodManager imm)
        {

            pView.ClearFocus();

            imm.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
        }

        private void AddOneHealth(ViewModel.LifeCounter ViewModel)
        {
            TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
            if (ActionBar.SelectedTab.Position == 0)
            {
                mainActivityModel.Life += 1;
                txtHelth.Text = mainActivityModel.Life.ToString();

            }
            else if (ActionBar.SelectedTab.Position == 1)
            {
                mainActivityModel.PoisonDamageCounter += 1;
                txtHelth.Text = mainActivityModel.PoisonDamageCounter.ToString();
            }
            else if (ActionBar.SelectedTab.Position == 2)
            {
                mainActivityModel.CommanderDamageCounter += 1;
                txtHelth.Text = mainActivityModel.CommanderDamageCounter.ToString();
            }
        }

        private void SubtractOneHealth(ViewModel.LifeCounter ViewModel)
        {
            TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
            if (ActionBar.SelectedTab.Position == 0)
            {
                mainActivityModel.Life -= 1;
                if (mainActivityModel.Life <= 0)
                {
                    mainActivityModel.Life = 0;
                    txtHelth.Text = (0).ToString();
                }
                else
                {
                    txtHelth.Text = (mainActivityModel.Life).ToString();
                }

            }
            else if (ActionBar.SelectedTab.Position == 1)
            {
                mainActivityModel.PoisonDamageCounter -= 1;
                if (mainActivityModel.PoisonDamageCounter <= 0)
                {
                    mainActivityModel.PoisonDamageCounter = 0;
                    txtHelth.Text = (0).ToString();
                }
                else
                {
                    txtHelth.Text = (mainActivityModel.PoisonDamageCounter).ToString();
                }

            }
            else if (ActionBar.SelectedTab.Position == 2)
            {
                mainActivityModel.CommanderDamageCounter -= 1;
                if (mainActivityModel.CommanderDamageCounter <= 0)
                {
                    mainActivityModel.CommanderDamageCounter = 0;
                    txtHelth.Text = (0).ToString();
                }
                else
                {
                    txtHelth.Text = (mainActivityModel.CommanderDamageCounter).ToString();
                }

            }

        }

        private void ResetHealth(ViewModel.LifeCounter ViewModel)
        {
            Dialog dia = new Dialog(this);
            dia = new Dialog(this);
            dia.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dia.SetContentView(Resource.Layout.layAreYouSure);

            Button cancelButton = dia.FindViewById<Button>(Resource.Id.btnCancelDiag);
            cancelButton.Click += delegate { dia.Dismiss(); };

            Button okayButton = dia.FindViewById<Button>(Resource.Id.btnOkayDiag);
            okayButton.Click += delegate
            {
                TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                if (ActionBar.SelectedTab.Position == 0)
                {
                    mainActivityModel.Life = 40;
                    txtHelth.Text = mainActivityModel.Life.ToString();

                }
                else if (ActionBar.SelectedTab.Position == 1)
                {
                    mainActivityModel.PoisonDamageCounter = 0;
                    txtHelth.Text = mainActivityModel.PoisonDamageCounter.ToString();
                }
                else if (ActionBar.SelectedTab.Position == 2)
                {
                    mainActivityModel.CommanderDamageCounter = 0;
                    txtHelth.Text = mainActivityModel.CommanderDamageCounter.ToString();
                }

                dia.Dismiss();
            };
            dia.Show();
        }

        private void CheckIfLost(int selectedTab)
        {


        }

        private ViewModel.LifeCounter Translate(EDHMTGDataAccess.Model.LifeCounter model)
        {
            ViewModel.LifeCounter returnView = new ViewModel.LifeCounter();
            returnView.CommanderDamageCounter = model.CommanderDamageCounter;
            returnView.CreateDate = model.CreateDate;
            returnView.Deleted = model.Deleted;
            returnView.Id = model.Id;
            returnView.Inserted = model.Inserted;
            returnView.Life = model.Life;
            returnView.PoisonDamageCounter = model.PoisonDamageCounter;
            returnView.Updated = model.Updated;
            returnView.UpdateDate = model.UpdateDate;
            returnView.User = model.User;

            return returnView;
        }

        private EDHMTGDataAccess.Model.LifeCounter Translate(ViewModel.LifeCounter dataModel)
        {
            EDHMTGDataAccess.Model.LifeCounter model = new EDHMTGDataAccess.Model.LifeCounter();
            model.CommanderDamageCounter = dataModel.CommanderDamageCounter;
            model.CreateDate = dataModel.CreateDate;
            model.Deleted = dataModel.Deleted;
            model.Id = dataModel.Id;
            model.Inserted = dataModel.Inserted;
            model.Life = dataModel.Life;
            model.PoisonDamageCounter = dataModel.PoisonDamageCounter;
            model.Updated = dataModel.Updated;
            model.UpdateDate = dataModel.UpdateDate;
            model.User = dataModel.User;

            return model;
        }
    }
}

