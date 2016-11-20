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
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LifeCounter);
            ActionBar.Tab tab = ActionBar.NewTab();
            tab.SetText("Life");
            
            tab.TabSelected += (sender, args) =>
            {
                SetContentView(Resource.Layout.LifeCounter);
                SetupLifeMainView();
            };
            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            tab.SetText("Poison");

            tab.TabSelected += (sender, args) =>
            {
                SetContentView(Resource.Layout.layPoison);
            };
            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            
            tab.SetText("Commander");

            tab.TabSelected += (sender, args) =>
            {
                SetContentView(Resource.Layout.layCommander);
            };
            ActionBar.AddTab(tab);
            ActionBar.Title = "EDH MTG Life Counter";
        }

        private void SetupLifeMainView()
        {
            Button addFiveButton = FindViewById<Button>(Resource.Id.btnAddAmount);
            Button addOneButton = FindViewById<Button>(Resource.Id.btnAddOne);
            Button subtractOneButton = FindViewById<Button>(Resource.Id.btnSubtractOne);
            Button reset = FindViewById<Button>(Resource.Id.btnReset);

            addFiveButton.Click += delegate { AddFiveHealth(); };
            addOneButton.Click += delegate { AddOneHealth(); };
            subtractOneButton.Click += delegate { SubtractOneHealth(); };

            reset.Click += delegate { ResetHealth(); };
        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }
        private void AddFiveHealth()
        {
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
            addAmountButton.Click += delegate {
                amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
                TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                txtHelth.Text = (Convert.ToInt32(txtHelth.Text) + Convert.ToInt32(amount.Text)).ToString();
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

        private void AddOneHealth()
        {
            TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
            txtHelth.Text = (Convert.ToInt32(txtHelth.Text) + 1).ToString();
        }

        private void SubtractOneHealth()
        {
            TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
            int currentHealth = (Convert.ToInt32(txtHelth.Text) - 1);
            if (currentHealth <= 0)
            {
                txtHelth.Text = (0).ToString();
            }
            else
            {
                txtHelth.Text = (currentHealth).ToString();
            }
        }

        private void ResetHealth()
        {
            Dialog dia = new Dialog(this);
            dia = new Dialog(this);
            dia.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dia.SetContentView(Resource.Layout.layAreYouSure);

            Button cancelButton = dia.FindViewById<Button>(Resource.Id.btnCancelDiag);
            cancelButton.Click += delegate { dia.Dismiss(); };

            Button okayButton = dia.FindViewById<Button>(Resource.Id.btnOkayDiag);
            okayButton.Click += delegate {
                TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                txtHelth.Text = 40.ToString();
                dia.Dismiss();
            };
            dia.Show();
        }
    }
}

