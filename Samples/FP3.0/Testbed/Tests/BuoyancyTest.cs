﻿using FarseerGames.FarseerPhysics.Controllers;
using FarseerGames.FarseerPhysics.Interfaces;
using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarseerPhysics.TestBed.Tests
{
    public class BuoyancyTest : Test
    {
        private BuoyancyTest()
        {
            //Make a box
            //Bottom
            Body ground = World.CreateBody();
            Vertices edge = PolygonTools.CreateEdge(new Vector2(-20.0f, 0.0f), new Vector2(20.0f, 0.0f));
            PolygonShape shape = new PolygonShape(edge);
            ground.CreateFixture(shape);

            //Left side
            shape.Set(PolygonTools.CreateEdge(new Vector2(-20.0f, 0.0f), new Vector2(-20.0f, 15.0f)));
            ground.CreateFixture(shape);

            //Right side
            shape.Set(PolygonTools.CreateEdge(new Vector2(20.0f, 0.0f), new Vector2(20.0f, 15.0f)));
            ground.CreateFixture(shape);

            //Buoyancy controller
            _aabbContainer = new AABBFluidContainer(40, 10, new Vector2(-20, 0));

            _waveContainer = new WaveController();
            _waveContainer.Position = new Vector2(-20, 0);
            _waveContainer.Width = 40;
            _waveContainer.Height = 10;
            _waveContainer.NodeCount = 100;
            _waveContainer.DampingCoefficient = 0.98f;
            _waveContainer.Frequency = .001f;

            _waveContainer.WaveGeneratorMax = 4;
            _waveContainer.WaveGeneratorMin = 2;
            _waveContainer.WaveGeneratorStep = 0.1f;

            _waveContainer.Initialize();

            FluidDragController buoyancyController = new FluidDragController(_waveContainer, 4f, 0.98f, 0.002f, World.Gravity);
            buoyancyController.Entry +=EntryEventHandler;

            Vector2 offset = new Vector2(5, 0);

            //Bunch of balls
            for (int i = 0; i < 4; i++)
            {
                Body circleBody = World.CreateBody();
                circleBody.BodyType = BodyType.Dynamic;
                circleBody.Position = new Vector2(-7, 1) + offset * i;

                CircleShape circleShape = new CircleShape(1, 1);

                buoyancyController.AddGeom(circleBody.CreateFixture(circleShape));
            }

            World.AddController(buoyancyController);
        }

        public override void Update(Framework.Settings settings)
        {
            //DebugView.DrawAABB(ref _aabbContainer.AABB, Color.Wheat);
            //DebugView.DrawAABB(ref _waveContainer._aabb, Color.Wheat);
            //_waveContainer.Update(settings.Hz);
            DebugView.DrawWaveContainer(_waveContainer);
            base.Update(settings);
        }

        public void EntryEventHandler(Fixture geom, Vertices verts)
        {
            for (int i = 0; i < verts.Count; i++)
            {
                Vector2 vel, point = verts[i];
                vel = geom.Body.GetLinearVelocityFromWorldPoint(point);

                _waveContainer.Disturb(verts[i].X, (vel.Y * geom.Body.Mass) / (100.0f * geom.Body.Mass));
            }
        }

        private AABBFluidContainer _aabbContainer;
        private WaveController _waveContainer;

        public static Test Create()
        {
            return new BuoyancyTest();
        }
    }
}