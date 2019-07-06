using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V4.App;
using Android;
using Android.Content;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Square.Picasso;
using FragmentTransaction = Android.App.FragmentTransaction;

namespace Routes
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button loadImageBtn, searchImageBtn;
        private ImageView imgView;
        //private TextView pictureInfo;
        private string imagePath;
        private List<Painting> all_paintings = ContentLoader.load();
        private Route currentRoute = ContentLoader.getRoute1();

        public const string key = "3fa1f65516424aefa6af2a2abaf63308";
        public readonly HttpClient client = new HttpClient
        {
            DefaultRequestHeaders = { { "Ocp-Apim-Subscription-Key", key } }
        };
        public string baseurl = "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/analyze";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.registration);

            Button buttonSignUp = FindViewById<Button>(Resource.Id.buttonSignUp); //открытие окна зарег
            
            buttonSignUp.Click += (object sender, EventArgs args) =>
            {
                /*FragmentTransaction transaction = FragmentManager.BeginTransaction();
                reg_dialogwindow reg_Dialogwindow = new reg_dialogwindow();
                reg_Dialogwindow.Show(transaction, "dialog fragment");*/
                SetContentView(Resource.Layout.dialog_signup);
                loadButtons();
            };

            Button buttonLogIn = FindViewById<Button>(Resource.Id.buttonLogIn); //открытие окна войти

            buttonLogIn.Click += (object sender, EventArgs args) =>
            {
                /*FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialog_signin dialog_Signin = new dialog_signin();
                dialog_Signin.Show(transaction, "dialog fragment");*/
                SetContentView(Resource.Layout.dialog_signin);
                loadButtons();
            };
            
            loadButtons();
            
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 57);
        }
        void loadButtons()
        {
            loadImageBtn = (Button)FindViewById(Resource.Id.button4);
            if (loadImageBtn != null)
                loadImageBtn.Click += delegate {

                    Intent image = new Intent();
                    image.SetType("image/*");
                    image.SetAction(Intent.ActionGetContent);
                    StartActivityForResult(Intent.CreateChooser(image, "choose"), 57);
                };
            searchImageBtn = (Button)FindViewById(Resource.Id.button5);
            if (searchImageBtn != null)
                searchImageBtn.Click += delegate
                {
                    Analyze(imagePath);
                };

            Button buttonSignIn = (Button)FindViewById(Resource.Id.buttonSignIn); //войти
            if (buttonSignIn != null)
                buttonSignIn.Click += delegate
                {
                    SetContentView(Resource.Layout.profile);
                    loadButtons();
                };

            Button button_SignUp = (Button)FindViewById(Resource.Id.button_SignUp); //зарег
            if (button_SignUp != null)
                button_SignUp.Click += delegate
                {
                    SetContentView(Resource.Layout.profile);
                    loadButtons();
                };

            Button buttonCreateRoute = (Button)FindViewById(Resource.Id.buttonCreateRoute); //открыть создать маршрут
            if (buttonCreateRoute != null)
                buttonCreateRoute.Click += delegate
                {
                    SetContentView(Resource.Layout.create);
                    loadButtons();
                };

            Button buttonMyRoutes = (Button)FindViewById(Resource.Id.buttonMyRoutes); //открыть мои маршруты
            if (buttonMyRoutes != null)
                buttonMyRoutes.Click += delegate
                {
                    SetContentView(Resource.Layout.my_routes);
                    loadButtons();
                };

            Button buttonPopular = (Button)FindViewById(Resource.Id.buttonPopular); //открыть рек маршруты
            if (buttonPopular != null)
                buttonPopular.Click += delegate
                {
                    SetContentView(Resource.Layout.popular);
                    loadButtons();
                };

            Button buttonProfile = (Button)FindViewById(Resource.Id.buttonProfile); //открыть профиль
            if (buttonProfile != null)
                buttonProfile.Click += delegate
                {
                    SetContentView(Resource.Layout.profile);
                    loadButtons();
                };

            Button filterButton = (Button)FindViewById(Resource.Id.buttonFilters); //открыть страницу фильтров
            if (filterButton != null)
                filterButton.Click += delegate
                {
                    SetContentView(Resource.Layout.filter);
                    loadButtons();
                };

            Button btnAdd = (Button)FindViewById(Resource.Id.btnAdd); //применить фильтр
            if (btnAdd != null)
                btnAdd.Click += delegate
                {
                    SetContentView(Resource.Layout.create);
                    loadButtons();
                };

            Button buttonCreate = (Button)FindViewById(Resource.Id.buttonCreate); //создать новый маршрут
            if (buttonCreate != null)
                buttonCreate.Click += delegate
                {
                    SetContentView(Resource.Layout.after_search);
                    loadButtons();
                };

            Button btnInfo_Cla = (Button)FindViewById(Resource.Id.btnInfo_Cla); //информация о маршруте классицизм
            if (btnInfo_Cla != null)
                btnInfo_Cla.Click += delegate
                {
                    SetContentView(Resource.Layout.route_klassicism);
                    loadButtons();
                };

            Button btnInfo_First = (Button)FindViewById(Resource.Id.btnInfo_First); //информация о обзорном маршруте
            if (btnInfo_First != null)
                btnInfo_First.Click += delegate
                {
                    SetContentView(Resource.Layout.route_first);
                    loadButtons();
                };

            Button btnInfo_Rom = (Button)FindViewById(Resource.Id.btnInfo_Rom); //информация о маршруте романтизм
            if (btnInfo_Rom != null)
                btnInfo_Rom.Click += delegate
                {
                    SetContentView(Resource.Layout.route_romantism);
                    loadButtons();
                };

            Button button_Search = (Button)FindViewById(Resource.Id.button_Search); //поиск
            if (button_Search != null)
                button_Search.Click += delegate
                {
                    SetContentView(Resource.Layout.search);
                    loadButtons();
                };

            Button button_Next = (Button)FindViewById(Resource.Id.button_Next); // next picture
            if (button_Next != null)
                button_Next.Click += delegate
                {
                    next();
                    //loadButtons();
                };

            Button klassicismStart = (Button)FindViewById(Resource.Id.klassicismStart);
            if (klassicismStart != null)
                klassicismStart.Click += delegate
                {
                    currentRoute = ContentLoader.getRoute2();
                    SetContentView(Resource.Layout.activity_main);
                    loadButtons();
                    next();
                };

            Button romantismStart = (Button)FindViewById(Resource.Id.romantismStart);
            if (romantismStart != null)
                romantismStart.Click += delegate
                {
                    currentRoute = ContentLoader.getRoute3();
                    SetContentView(Resource.Layout.activity_main);
                    loadButtons();
                    next();
                };

            Button firstStart = (Button)FindViewById(Resource.Id.firstStart);
            if (firstStart != null)
                firstStart.Click += delegate
                {
                    currentRoute = ContentLoader.getRoute1();
                    SetContentView(Resource.Layout.activity_main);
                    loadButtons();
                    next();
                };
        }
        void next()
        {
            if (currentRoute.now == currentRoute.paintings.Count)
            {
                SetContentView(Resource.Layout.route_finished);
                TextView rname = (TextView)FindViewById(Resource.Id.textView1);
                TextView rdesc = (TextView)FindViewById(Resource.Id.textView2);
                rname.Text = currentRoute.name;
                rdesc.Text = "";
                loadButtons();
                return;
            }

            Painting cur_pic = currentRoute.paintings[currentRoute.now];
            TextView name = (TextView)FindViewById(Resource.Id.textView1);
            TextView desc = (TextView)FindViewById(Resource.Id.textView2);
            TextView next_place = (TextView)FindViewById(Resource.Id.textView3);
            ImageView iv = (ImageView)FindViewById(Resource.Id.imageView1);
            
            //Picasso.With(Application.Context).Load(Android.Net.Uri.Parse(cur_pic.picture_url)).Into(iv);
            name.Text = cur_pic.name + " - " + cur_pic.author;
            desc.Text = cur_pic.info;
            if (currentRoute.now < currentRoute.paintings.Count - 1)
            {
                next_place.Text = "Следующая картина находится в зале " + currentRoute.paintings[currentRoute.now + 1].place;
            } else
            {
                next_place.Text = "Это последняя картина";
            }

            currentRoute.now++;
            //loadButtons();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                string path = ActualPath.GetActualPathFromFile(data.Data, this);
                ImageView imgView = (ImageView)FindViewById(Resource.Id.imageView1);
                imgView.SetImageURI(data.Data);
                //Picasso.With(Application.Context).Load("http://i.imgur.com/DvpvklR.png").Into(imgView);
                imagePath = path;
            }
        }
        async Task Analyze(string path)
        {
            string param = "visualFeatures=Description";
            string url = baseurl + "?" + param;
            HttpResponseMessage response;
            byte[] img = GetBytesFromImage(path);
            using (ByteArrayContent content = new ByteArrayContent(img))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
            }
            string resp = await response.Content.ReadAsStringAsync();
            JToken tmp = JToken.Parse(resp);
            SetContentView(Resource.Layout.paint_info);
            loadButtons();
            TextView pictureName = (TextView)FindViewById(Resource.Id.textView56);
            TextView pictureInfo = (TextView)FindViewById(Resource.Id.textView57);
            //ImageView img_view = (ImageView)FindViewById(Resource.Id.imageView57);
            List<string> taglist = new List<string>(tmp["description"]["tags"].ToObject<List<string>>());
            Painting pic = findBestPaint(taglist);
            pictureName.Text = pic.name + " - " + pic.author;
            pictureInfo.Text = pic.info;
            //Picasso.With(Application.Context).Load(Android.Net.Uri.Parse(path)).Into(img_view);
            //img_view.SetImageBitmap(pic.GetImageBitmap());
            //img_view.SetImageURI(Android.Net.Uri.Parse(path));
        }
        private byte[] GetBytesFromImage(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(stream);
                return reader.ReadBytes((int)stream.Length);
            }
        }
        private Painting findBestPaint(List<string> tags)
        {
            int bestcount = -1;
            Painting bestp = all_paintings[0];
            foreach (Painting p in all_paintings)
            {
                int tmp = p.countTags(tags);
                if (tmp > bestcount)
                {
                    bestcount = tmp;
                    bestp = p;
                }
            }
            return bestp;
        }
    }
}