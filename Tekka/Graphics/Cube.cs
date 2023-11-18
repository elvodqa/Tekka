﻿using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Tekka.Graphics;

/*
    public override void Render(GL Gl, Camera camera, LightSource[] lights)
    {
        VaoCube.Bind();
        DefaultShader.Use();
        
        
        DefaultShader.SetUniform("uModel", Transform.Model);
        DefaultShader.SetUniform("uView", camera.GetViewMatrix());
        DefaultShader.SetUniform("uProjection", camera.GetProjectionMatrix());
        DefaultShader.SetUniform("viewPos", camera.Position);
        DefaultShader.SetUniform("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
        DefaultShader.SetUniform("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
        DefaultShader.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        DefaultShader.SetUniform("material.shininess", 32.0f);

        DefaultShader.SetUniform("light1.position", lights[0].Position);
        DefaultShader.SetUniform("light1.diffuse", lights[0].DiffuseColor);
        DefaultShader.SetUniform("light1.specular", lights[0].SpecularColor);
        
        DefaultShader.SetUniform("light2.position", lights[1].Position);
        DefaultShader.SetUniform("light2.diffuse", lights[1].DiffuseColor);
        DefaultShader.SetUniform("light2.specular", lights[1].SpecularColor);
        
        DefaultShader.SetUniform("light3.position", lights[2].Position);
        DefaultShader.SetUniform("light3.diffuse", lights[2].DiffuseColor);
        DefaultShader.SetUniform("light3.specular", lights[2].SpecularColor);
        
        DefaultShader.SetUniform("light4.position", lights[3].Position);
        DefaultShader.SetUniform("light4.diffuse", lights[3].DiffuseColor);
        DefaultShader.SetUniform("light4.specular", lights[3].SpecularColor);
        
        DefaultShader.SetUniform("world_color", new Vector3(0.4f, 0.4f, 0.4f));
        
        Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        
        VaoCube.Unbind();
        DefaultShader.Unbind();
*/