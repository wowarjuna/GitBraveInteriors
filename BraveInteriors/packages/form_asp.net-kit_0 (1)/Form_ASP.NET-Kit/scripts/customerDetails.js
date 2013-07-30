	
// run when checkbox is clicked to synchronise the delivery details with billing details
function IsDeliverySame_clicked(calledByCode) {

    if (document.customerform.IsDeliverySame.checked) {

        document.customerform.txtDeliveryFirstname.value = "";
        document.customerform.txtDeliveryFirstname.className = "inputBoxDisable";
        document.customerform.txtDeliveryFirstname.disabled = true;
        
        document.customerform.txtDeliverySurname.value = "";
        document.customerform.txtDeliverySurname.className = "inputBoxDisable";
        document.customerform.txtDeliverySurname.disabled = true;
        
        document.customerform.txtDeliveryAddressLine1.value = "";
        document.customerform.txtDeliveryAddressLine1.className = "inputBoxDisable";
        document.customerform.txtDeliveryAddressLine1.disabled = true;

        document.customerform.txtDeliveryAddressLine2.value = "";
        document.customerform.txtDeliveryAddressLine2.className = "inputBoxDisable";
        document.customerform.txtDeliveryAddressLine2.disabled = true; 

        document.customerform.txtDeliveryCity.value = "";
        document.customerform.txtDeliveryCity.className = "inputBoxDisable";
        document.customerform.txtDeliveryCity.disabled = true;

        document.customerform.txtDeliveryPostCode.value = "";
        document.customerform.txtDeliveryPostCode.className = "inputBoxDisable";
        document.customerform.txtDeliveryPostCode.disabled = true;

        document.customerform.txtDeliveryCountry.value = "";
        document.customerform.txtDeliveryCountry.className = "inputBoxDisable";
        document.customerform.txtDeliveryCountry.disabled = true;

        document.customerform.txtDeliveryState.value = "";
        document.customerform.txtDeliveryState.className = "inputBoxDisable";
        document.customerform.txtDeliveryState.disabled = true;

        document.customerform.txtDeliveryPhone.value = "";
        document.customerform.txtDeliveryPhone.className = "inputBoxDisable";
        document.customerform.txtDeliveryPhone.disabled = true;
    } 
    else 
    {
        document.customerform.txtDeliveryFirstname.disabled = false;
        document.customerform.txtDeliveryFirstname.className = "inputBoxEnable";
        document.customerform.txtDeliverySurname.disabled = false;
        document.customerform.txtDeliverySurname.className = "inputBoxEnable";
        document.customerform.txtDeliveryAddressLine1.disabled = false;
        document.customerform.txtDeliveryAddressLine1.className = "inputBoxEnable";
        document.customerform.txtDeliveryAddressLine2.disabled = false;
        document.customerform.txtDeliveryAddressLine2.className = "inputBoxEnable";
        document.customerform.txtDeliveryCity.disabled = false;
        document.customerform.txtDeliveryCity.className = "inputBoxEnable";
        document.customerform.txtDeliveryPostCode.disabled = false;
        document.customerform.txtDeliveryPostCode.className = "inputBoxEnable";
        document.customerform.txtDeliveryCountry.disabled = false;
        document.customerform.txtDeliveryCountry.className = "inputBoxEnable";
        document.customerform.txtDeliveryState.disabled = false;
        document.customerform.txtDeliveryState.className = "inputBoxEnable";
        document.customerform.txtDeliveryPhone.disabled = false;
        document.customerform.txtDeliveryPhone.className = "inputBoxEnable";
        if(calledByCode!=true) {
        	document.customerform.txtDeliveryFirstname.focus();
    	}
    }
}

