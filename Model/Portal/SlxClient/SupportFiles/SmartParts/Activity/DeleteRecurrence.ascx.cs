using System;
using System.Collections.Generic;
using Sage.Platform.EntityBinding;
using Sage.Platform.WebPortal.Binding;
using Sage.Platform.WebPortal.SmartParts;
using Sage.SalesLogix.Activity;

public partial class SmartParts_Activity_DeleteRecurrence : EntityBoundSmartPartInfoProvider
{
    private Activity Activity
    {
        get { return (Activity)BindingSource.Current; }
    }

    private ActivityParameters _Params;
    private ActivityParameters Params
    {
        get
        {
            if (_Params != null)
                return _Params;
            _Params = new ActivityParameters(
                (Dictionary<string, string>)AppContext["ActivityParameters"] ?? new Dictionary<string, string>());
            return _Params;
        }
    }

    public override Type EntityType
    {
        get { return typeof(Sage.Entity.Interfaces.IActivity); }
    }

    protected override void OnAddEntityBindings()
    {
        EntityBindingSource bs = BindingSource;
        bs.Bindings.Add(new WebEntityBinding("StartDate", StartDate, "DateTimeValue"));
        bs.Bindings.Add(new WebEntityBinding("ContactName", Contact, "Text"));
        bs.Bindings.Add(new WebEntityBinding("AccountName", Account, "Text"));
        bs.Bindings.Add(new WebEntityBinding("OpportunityName", Opportunity, "Text"));
    }

    protected override void OnFormBound()
    {
        base.OnFormBound();

        StartDate.DateTimeValue = GetStartDate();

        if (Activity.Timeless)
        {
            StartDate.DisplayTime = false;
            StartDate.Timeless = true;
            StartDate.Text = StartDate.Text + " (" + GetLocalResourceObject("Const_Timeless") + ")";
        }

        if (!String.IsNullOrEmpty(Activity.LeadId))
        {
            OpportunityName.Visible = false;
            Opportunity.Visible = false;
            ContactName.Text = GetLocalResourceObject("LeadName").ToString();
            AccountName.Text = GetLocalResourceObject("CompanyName").ToString();
            Contact.Text = Activity.LeadName;
        }
        else
        {
            OpportunityName.Visible = true;
            Opportunity.Visible = true;
            ContactName.Text = GetLocalResourceObject("ContactName.Text").ToString();
            AccountName.Text = GetLocalResourceObject("AccountName.Text").ToString();
            Contact.Text = Activity.ContactName;
        }
    }

    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();
        btnContinue.Click += DialogService.CloseEventHappened;
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (rblRecurrenceType.SelectedValue == "series")
        {
            Activity.Delete();
        }
        else
        {
            Activity occurrence = Activity.RecurrencePattern.GetOccurrence(GetStartDate());
            occurrence.Delete();
        }
    }

    private DateTime GetStartDate()
    {
        return Params.RecurDate.HasValue ? Params.RecurDate.Value : Activity.StartDate;
    }
}
