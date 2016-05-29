﻿/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects;


namespace Intersect_Editor.Forms.Editors
{
    public partial class frmSwitchVariable : Form
    {
        private List<DatabaseObject> _changed = new List<DatabaseObject>();
        private DatabaseObject _editorItem = null;
        public frmSwitchVariable()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.PlayerSwitch)
            {
                InitEditor();
                if (_editorItem != null && !PlayerSwitchBase.GetObjects().Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObject.PlayerVariable)
            {
                InitEditor();
                if (_editorItem != null && !PlayerVariableBase.GetObjects().Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObject.ServerSwitch)
            {
                InitEditor();
                if (_editorItem != null && !ServerSwitchBase.GetObjects().Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObject.ServerVariable)
            {
                InitEditor();
                if (_editorItem != null && !ServerVariableBase.GetObjects().Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (rdoPlayerSwitch.Checked)
            {
                PacketSender.SendCreateObject(GameObject.PlayerSwitch);
            }
            else if (rdoPlayerVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObject.PlayerVariable);
            }
            else if (rdoGlobalSwitches.Checked)
            {
                PacketSender.SendCreateObject(GameObject.ServerSwitch);
            }
            else if (rdoGlobalVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObject.ServerVariable);
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                _editorItem.RestoreBackup();
                UpdateEditor();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in _changed)
            {
                item.RestoreBackup();
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            foreach (var item in _changed)
            {
                PacketSender.SendSaveObject(item);
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstObjects_Click(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                DatabaseObject obj = null;
                if (rdoPlayerSwitch.Checked)
                {
                    obj = PlayerSwitchBase.GetSwitch(Database.GameObjectIdFromList(GameObject.PlayerSwitch, lstObjects.SelectedIndex));
                }
                else if (rdoPlayerVariables.Checked)
                {
                    obj = PlayerVariableBase.GetVariable(Database.GameObjectIdFromList(GameObject.PlayerVariable, lstObjects.SelectedIndex));
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    obj = ServerSwitchBase.GetSwitch(Database.GameObjectIdFromList(GameObject.ServerSwitch, lstObjects.SelectedIndex));
                }
                else if (rdoGlobalVariables.Checked)
                {
                    obj = ServerVariableBase.GetVariable(Database.GameObjectIdFromList(GameObject.ServerVariable, lstObjects.SelectedIndex));
                }
                if (obj != null)
                {
                    _editorItem = obj;
                    if (!_changed.Contains(obj))
                    {
                        _changed.Add(obj);
                        obj.MakeBackup();
                    }
                }
            }
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstObjects.Items.Clear();
            grpEditor.Hide();
            cmbSwitchValue.Hide();
            txtVariableVal.Hide();
            if (rdoPlayerSwitch.Checked)
            {
                lstObjects.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerSwitch));
            }
            else if (rdoPlayerVariables.Checked)
            {
                lstObjects.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerVariable));
            }
            else if (rdoGlobalSwitches.Checked)
            {
                for (int i = 0; i < ServerSwitchBase.ObjectCount(); i++)
                {
                    var swtch = ServerSwitchBase.GetSwitch(Database.GameObjectIdFromList(GameObject.ServerSwitch, i));
                    lstObjects.Items.Add(swtch.Name + "  =  " + swtch.Value.ToString());
                }
            }
            else if (rdoGlobalVariables.Checked)
            {
                for (int i = 0; i < ServerVariableBase.ObjectCount(); i++)
                {
                    var var = ServerVariableBase.GetVariable(Database.GameObjectIdFromList(GameObject.ServerVariable, i));
                    lstObjects.Items.Add(var.Name + "  =  " + var.Value.ToString());
                }
            }
            UpdateEditor();
        }

        private void frmSwitchVariable_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void rdoPlayerSwitch_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem = null;
            InitEditor();
        }

        private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem = null;
            InitEditor();
        }

        private void rdoGlobalSwitches_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem = null;
            InitEditor();
        }

        private void rdoGlobalVariables_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem = null;
            InitEditor();
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                grpEditor.Show();
                lblValue.Hide();
                if (rdoPlayerSwitch.Checked)
                {
                    lblObject.Text = "Player Switch";
                    txtObjectName.Text = ((PlayerSwitchBase)_editorItem).Name;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    lblObject.Text = "Player Variable";
                    txtObjectName.Text = ((PlayerVariableBase)_editorItem).Name;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    lblObject.Text = "Server Switch";
                    txtObjectName.Text = ((ServerSwitchBase)_editorItem).Name;
                    cmbSwitchValue.Show();
                    cmbSwitchValue.SelectedIndex = cmbSwitchValue.Items.IndexOf(((ServerSwitchBase)_editorItem).Value.ToString());
                }
                else if (rdoGlobalVariables.Checked)
                {
                    lblObject.Text = "Server Variable";
                    txtObjectName.Text = ((ServerVariableBase) _editorItem).Name;
                    txtVariableVal.Show();
                    txtVariableVal.Text = ((ServerVariableBase)_editorItem).Value.ToString();
                }
            }
            else
            {
                grpEditor.Hide();
            }
        }

        private void cmbSwitchValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoGlobalSwitches.Checked)
                {
                    var obj = ServerSwitchBase.GetSwitch(Database.GameObjectIdFromList(GameObject.ServerSwitch, lstObjects.SelectedIndex));
                    obj.Value = Convert.ToBoolean(cmbSwitchValue.SelectedIndex);
                    UpdateSelection();
                }
            }
        }

        private void UpdateSelection()
        {
            if (lstObjects.SelectedIndex > -1)
            {
                grpEditor.Show();
                lblValue.Hide();
                if (rdoPlayerSwitch.Checked)
                {
                    var obj = PlayerSwitchBase.GetSwitch(Database.GameObjectIdFromList(GameObject.PlayerSwitch, lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.GetVariable(Database.GameObjectIdFromList(GameObject.PlayerVariable, lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    var obj = ServerSwitchBase.GetSwitch(Database.GameObjectIdFromList(GameObject.ServerSwitch, lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] =  obj.Name + "  =  " + obj.Value.ToString();
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.GetVariable(Database.GameObjectIdFromList(GameObject.ServerVariable, lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name + "  =  " + obj.Value.ToString();
                }
            }
        }

        private void txtVariableVal_TextChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoGlobalVariables.Checked)
                {
                    int readInt = 0;
                    if (int.TryParse(txtVariableVal.Text, out readInt))
                    {
                        var obj = ServerVariableBase.GetVariable(Database.GameObjectIdFromList(GameObject.ServerVariable, lstObjects.SelectedIndex));
                        obj.Value = readInt;
                        UpdateSelection();
                    }
                }
            }
        }

        private void txtObjectName_TextChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoPlayerSwitch.Checked)
                {
                    var obj = PlayerSwitchBase.GetSwitch(Database.GameObjectIdFromList(GameObject.PlayerSwitch, lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.GetVariable(Database.GameObjectIdFromList(GameObject.PlayerVariable, lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    var obj = ServerSwitchBase.GetSwitch(Database.GameObjectIdFromList(GameObject.ServerSwitch, lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.GetVariable(Database.GameObjectIdFromList(GameObject.ServerVariable, lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                UpdateSelection();
            }
        }
    }
}