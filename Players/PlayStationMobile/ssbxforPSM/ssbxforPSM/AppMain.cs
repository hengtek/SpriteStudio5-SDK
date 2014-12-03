/* PlayStation(R)Mobile SDK 2.00.00
 * Copyright (C) 2014 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.IO;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.UI;

namespace ss
{
	public class AppMain
	{
		private static GraphicsContext graphics;		
		
		//アニメ操作用
		private static GamePadData gamePadData;
		private static int frame_count;
		private static bool anime_stop;
		private static bool press;
		private static int motion_type;
		
		private static Label label1;
		private static Label label2;

		//アニメ再生用のプレイヤー
		private static ResourceManager resourcemaneger;		//リソースマネージャ
		private static Player player;						//プレイヤー
		
		public static void Main (string[] args)
		{
			Initialize ();

			while (true) {
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
			}
		}
		
		public static void Initialize ()
		{
			//グラフィックコンテキストの作成
			graphics = new GraphicsContext( 960, 544, PixelFormat.Rgba, PixelFormat.Depth16, MultiSampleMode.None );		
			
			//リソースマネージャの作成
			resourcemaneger = new ResourceManager();
			//ssbxの読み込み
			resourcemaneger.Load("/Application/resources/character_template1.xml");
				
			//プレイヤーの作成とssbxの割り当て
			player = new Player(graphics);
			player.SetAnimedata( resourcemaneger, "character_template1");
			
			//プレイヤーの設定
			player.SetPostion(500, 400);		//位置
			player.SetRotation(0);				//角度
			player.SetScale(0.5f, 0.5f);		//スケール
			player.SetAlpha(255);				//透明度
			
			//モーションを指定して再生します。
			player.Play("character_template_3head/stance");
			
			
			//時間計測表示
			String str;
			//UIの初期化
            UISystem.Initialize(graphics);
			//シーンの生成
            Scene scene=new Scene();
            UISystem.SetScene(scene,null);
			//ラベル作成
			label1=new Label();
			str = string.Format("xml load: {0} ms", resourcemaneger.GetLoadtime());
            label1.Text=str;
            label1.TextColor=new UIColor(1.0f,1.0f,1.0f,1.0f);
			label1.SetPosition(0.0f, 0.0f);
            scene.RootWidget.AddChildLast(label1);			

			label2=new Label();
			str = string.Format("", resourcemaneger.GetLoadtime());
            label2.Text=str;
            label2.TextColor=new UIColor(1.0f,1.0f,1.0f,1.0f);
			label2.SetPosition(0.0f, 20.0f);
            scene.RootWidget.AddChildLast(label2);			
		}
	
		public static void Update ()
		{
			//アニメーション更新
			player.Update();
			int maxframe = player.GetMaxFrame();	//アニメの総フレームを取得
			
			//再生フレームを取得（ポーズ中は設定する）
			if( anime_stop == false )
			{
				frame_count = player.GetFrame(); 
			}
			else
			{
				player.SetFrame(frame_count); 
			}
			
			//キー入力でアニメーションを操作する
			gamePadData = GamePad.GetData(0);
			if((gamePadData.Buttons & GamePadButtons.Left) != 0)
	        {
				if ( press == false )
				{
					frame_count--;
					if ( frame_count < 0 )
					{
						frame_count += maxframe;
					}
				}
				press = true;
	        }
	        else if((gamePadData.Buttons & GamePadButtons.Right) != 0)
	        {
				if ( press == false )
				{
					frame_count++;
					if ( frame_count >= maxframe )
					{
						frame_count -= maxframe;
					}
				}
				press = true;
	        }
	        else if((gamePadData.Buttons & GamePadButtons.Up) != 0)
	        {
				if ( press == false )
				{
					frame_count += 10;
					if ( frame_count >= maxframe )
					{
						frame_count -= maxframe;
					}
				}
				press = true;
	        }
	        else if((gamePadData.Buttons & GamePadButtons.Down) != 0)
	        {
				if ( press == false )
				{
					frame_count -= 10;
					if ( frame_count < 0 )
					{
						frame_count += maxframe;
					}
				}
				press = true;
	        }
	        else if((gamePadData.Buttons & GamePadButtons.Circle) != 0)
	        {
				//○ボタンでアニメのポーズレジューム
				if ( press == false )
				{
					if ( anime_stop == false )
					{
						anime_stop = true;
						player.Pause();		//アニメ停止
					}
					else
					{
						anime_stop = false;
						player.Resume();	//アニメ再生
					}
					
				}
				press = true;
	        }			
	        else if((gamePadData.Buttons & GamePadButtons.Cross) != 0)
	        {
				//×ボタンでアニメモーション変更
				if ( press == false )
				{
					motion_type++;
					if ( motion_type > 4 )
					{
						motion_type = 0;
					}
					switch( motion_type )
					{
					case 0:
						player.Play("character_template_3head/stance");
						break;
					case 1:
						player.Play("character_template_3head/attack1");
						break;
					case 2:
						player.Play("character_template_3head/defense");
						break;
					case 3:
						player.Play("character_template_3head/kick2");
						break;
					case 4:
						player.Play("character_template_3head/walk");
						break;
					default:
						break;
					}
					frame_count = 0;
				}
				press = true;
	        }			
			else
			{
				press = false;
			}
			//再生フレームの表示
			string str;
			str = string.Format("frame: {0}/{1}", frame_count, maxframe);
            label2.Text=str;
			
			//タッチ情報の取得
            List<TouchData> touchData=Touch.GetData(0);
            
            //UIの更新
            UISystem.Update(touchData);			//UIの更新
		}

		public static void Render ()
		{
			graphics.Clear();

			//プレイヤー描画
			player.Draw();

			//UIの描画	
            UISystem.Render();
			
			//画面更新
			graphics.SwapBuffers();	
		}
	}
}
