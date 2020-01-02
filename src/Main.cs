// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();
	private AudioStream templeAudio = GD.Load<AudioStream>("res://assets/temple.ogg");

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		var env = GetNode<WorldEnvironment>("sky").Environment;
		env.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		env.BackgroundMode = Godot.Environment.BGMode.Sky;
		env.BackgroundSky = new PanoramaSky() { Panorama = ((Texture)GD.Load("res://assets/temple.hdr")) };
		env.BackgroundSkyRotationDegrees = new Vector3(0, 0, 0);
		env.BackgroundSkyCustomFov = 100;

		InitSound();
		AddChild(Audio);

		templeAudio.Play(Audio);
		Audio.Seek((float)GD.RandRange(0, templeAudio.GetLength()));

		var particles = new CPUParticles() {
			Mesh = new QuadMesh(),
			EmissionShape = CPUParticles.EmissionShapeEnum.Sphere,
			Amount = 800,
			EmissionSphereRadius = 0.2f,
			Spread = 3,
			Gravity = new Vector3(0, 0, 0),
			InitialVelocity = 1,
			InitialVelocityRandom = 1,
			ScaleAmountRandom = 0.5f,
			Lifetime = 20,
			AngularVelocity = 30,
			AngularVelocityRandom = 1,
			LinearAccel = -0.01f,
			Scale = Scale * 0.15f,
			ScaleAmount = 0.5f,
			AngleRandom = 1,
			ColorRamp = new Gradient() {
				Offsets = new[] { 0, 0.5f, 1 },
				Colors = new[] { Color.FromHsv(0, 0, 0, 0), Color.FromHsv(0, 0, 0.7f), Color.FromHsv(0, 0, 0, 0) }
			}
		};

		AddChild(particles);
		particles.Translate(new Vector3(0, 6f, 0));
		particles.RotateZ(Mathf.Deg2Rad(90));

		var mat = new SpatialMaterial() {
			FlagsUnshaded = true,
			VertexColorUseAsAlbedo = true,
			FlagsTransparent = true,
			ParamsBlendMode = SpatialMaterial.BlendMode.Add,
			ParamsBillboardMode = SpatialMaterial.BillboardMode.Particles,
			AlbedoColor = Color.FromHsv(0, 0, 0.1f),
			AlbedoTexture = new NoiseTexture() {
				Noise = new OpenSimplexNoise() {
					Period = 40,
					Persistence = 0,
					Lacunarity = 0.1f,
				}
			}
		};
		particles.MaterialOverride = mat;
	}

	public override void _Process(float delta) {

	}

}
