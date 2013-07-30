	
// run when checkbox is clicked to synchronise the delivery details with billing details
function IsDeliverySame_clicked() {

    if (document.customerform.IsDeliverySame.checked) {

        document.customerform.txtDeliveryFirstnames.value = "";
        document.customerform.txtDeliveryFirstnames.className = "inputBoxDisable";
        document.customerform.txtDeliveryFirstnames.disabled = true;
        
        document.customerform.txtDeliverySurname.value = "";
        document.customerform.txtDeliverySurname.className = "inputBoxDisable";
        document.customerform.txtDeliverySurname.disabled = true;
        
        document.customerform.txtDeliveryAddress1.value = "";
        document.customerform.txtDeliveryAddress1.className = "inputBoxDisable";
        document.customerform.txtDeliveryAddress1.disabled = true;

        document.customerform.txtDeliveryAddress2.value = "";
        document.customerform.txtDeliveryAddress2.className = "inputBoxDisable";
        document.customerform.txtDeliveryAddress2.disabled = true; 

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
        document.customerform.txtDeliveryFirstnames.disabled = false;
        document.customerform.txtDeliveryFirstnames.className = "inputBoxEnable";
        document.customerform.txtDeliveryFirstnames.focus();
        document.customerform.txtDeliverySurname.disabled = false;
        document.customerform.txtDeliverySurname.className = "inputBoxEnable";
        document.customerform.txtDeliveryAddress1.disabled = false;
        document.customerform.txtDeliveryAddress1.className = "inputBoxEnable";
        document.customerform.txtDeliveryAddress2.disabled = false;
        document.customerform.txtDeliveryAddress2.className = "inputBoxEnable";
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
    }
}

