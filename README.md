# Game Tool
## Admob
### Setting 
1. Tạo AdsManager GameObject 

   Tạo 1 GameObject trong Scene và Add Compoment AdsManager.cs vào GameObject.

2. Setting Admob 
Vào Menu: GameTool -> Ads Setting.

![Ad Manager Panel](https://github.com/anhcon1995pt/UploadImageGameTool/blob/main/Screenshot%202022-07-19%20062239.png)

* Buttom CheckUpdate: 
Kiểm tra phiên bản mới nhất của Admob
* Buttom Update: 
Download và Import phiên bản mới nhất của Admob
* Is tag For Child: 
Quảng cáo có hướng đến trẻ em: True or False
* TimeBetweenInterstitial: 
Thời gian giãn cách giữa 2 làn quảng cáo Interstitial
* AutoLoadBannerOnStartup: 
Quảng cáo Banner sẽ hiển thị ngay khi Game chạy
* Android Ads Unit ID: 
Cài đặt các ID quảng cáo và các máy Test cho Android
* IOS Unit ID: 
Cài đặt các ID quảng cáo và các máy Test cho IOS
### Gọi Quảng Cáo: 
Thêm namespcae AC.GameTool.Ads vào Class
```C#
using AC.GameTool.Ads;
```
1. Banner
* Load và hiển thị Banner
```C#
 AdsManager.Instance.LoaddingAdsBanner();
```
* Hiên/Ẩn Banner đã Load
```C#
AdsManager.Instance.ShowAdsBanner(bool isShow);
ishow = True: Hiển thị Banner
ishow = False: Ẩn Banner
```
2. Interstital

Hiển thị quảng cáo Interstitial
```C#
AdsManager.Instance.ShowInterstitialAd();
```
3. Reward

Hiển thị quảng cáo Reward
```C#
AdsManager.Instance.ShowAdsReward(string placement, Action successedCallback, Action<string> failedCallback);
placement: Tên vị trí đặt quảng cáo(để debug or send Event)
successedCallback: Action để gọi lại khi người dùng xem hết quảng cáo và nhận thưởng
failedCallback: Action gọi khi quảng cáo bị lỗi: không load được, không hiển thị, lỗi mạng,...
VD:
AdsManager.Instance.ShowAdsReward("Collect X2", CollectX2Success, CollectX2Failed);
void CollectX2Success()
{
  //Thành công
  money += moneyBonus * 2;
}
void CollectX2Failed(String failedSmg)
{
  //Lỗi
  LogManager.Log("Da co loi: " + failedSmg);
}
```
4. Interstital Reward

Hiển thị quảng cáo Insterstitial Reward
```C#
AdsManager.Instance.ShowRewardedInterstitialAd(string placement, Action successedCallback, Action<string> failedCallback);
Thiết lập như Reward
```
5. App Open Ads

Khi tích vào Is Use Open App trong Windows "Ads Setting Manager" thì quảng cáo App Open sẽ tự động gọi mỗi khi người dùng mở App khác và quay trở lại Game.
Hoặc có thể gọi hàm sau để hiện thị App Open Ads thủ công (VD: khi mở game lần đầu tiên)
```C#
AdsManager.Instance.ShowAppOpenAdIfAvailable();
```
