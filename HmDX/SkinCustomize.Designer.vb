<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SkinCustomize
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SkinCustomize))
        Me.LblR = New System.Windows.Forms.Label()
        Me.NumR = New System.Windows.Forms.NumericUpDown()
        Me.NumG = New System.Windows.Forms.NumericUpDown()
        Me.LblG = New System.Windows.Forms.Label()
        Me.NumB = New System.Windows.Forms.NumericUpDown()
        Me.LblB = New System.Windows.Forms.Label()
        Me.BtnOK = New System.Windows.Forms.Button()
        CType(Me.NumR, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumG, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumB, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LblR
        '
        Me.LblR.AutoSize = True
        Me.LblR.Location = New System.Drawing.Point(8, 16)
        Me.LblR.Name = "LblR"
        Me.LblR.Size = New System.Drawing.Size(24, 21)
        Me.LblR.TabIndex = 10
        Me.LblR.Text = "R:"
        '
        'NumR
        '
        Me.NumR.Location = New System.Drawing.Point(38, 14)
        Me.NumR.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.NumR.Name = "NumR"
        Me.NumR.Size = New System.Drawing.Size(51, 29)
        Me.NumR.TabIndex = 12
        '
        'NumG
        '
        Me.NumG.Location = New System.Drawing.Point(128, 14)
        Me.NumG.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.NumG.Name = "NumG"
        Me.NumG.Size = New System.Drawing.Size(51, 29)
        Me.NumG.TabIndex = 22
        '
        'LblG
        '
        Me.LblG.AutoSize = True
        Me.LblG.Location = New System.Drawing.Point(98, 16)
        Me.LblG.Name = "LblG"
        Me.LblG.Size = New System.Drawing.Size(26, 21)
        Me.LblG.TabIndex = 20
        Me.LblG.Text = "G:"
        '
        'NumB
        '
        Me.NumB.Location = New System.Drawing.Point(218, 14)
        Me.NumB.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.NumB.Name = "NumB"
        Me.NumB.Size = New System.Drawing.Size(51, 29)
        Me.NumB.TabIndex = 32
        '
        'LblB
        '
        Me.LblB.AutoSize = True
        Me.LblB.Location = New System.Drawing.Point(188, 16)
        Me.LblB.Name = "LblB"
        Me.LblB.Size = New System.Drawing.Size(24, 21)
        Me.LblB.TabIndex = 30
        Me.LblB.Text = "B:"
        '
        'BtnOK
        '
        Me.BtnOK.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.BtnOK.Location = New System.Drawing.Point(91, 59)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(103, 40)
        Me.BtnOK.TabIndex = 100
        Me.BtnOK.Text = "确定"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'SkinRGB
        '
        Me.AcceptButton = Me.BtnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 21.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(284, 111)
        Me.Controls.Add(Me.BtnOK)
        Me.Controls.Add(Me.NumB)
        Me.Controls.Add(Me.LblB)
        Me.Controls.Add(Me.NumG)
        Me.Controls.Add(Me.LblG)
        Me.Controls.Add(Me.NumR)
        Me.Controls.Add(Me.LblR)
        Me.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SkinRGB"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "字体RGB颜色设置"
        CType(Me.NumR, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumG, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumB, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LblR As Label
    Friend WithEvents NumR As NumericUpDown
    Friend WithEvents NumG As NumericUpDown
    Friend WithEvents LblG As Label
    Friend WithEvents NumB As NumericUpDown
    Friend WithEvents LblB As Label
    Friend WithEvents BtnOK As Button
End Class
