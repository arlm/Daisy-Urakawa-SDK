package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.event.ChangeListener;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.ChangeNotifierImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.LanguageChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * 
 *
 */
public abstract class MediaAbstractImpl extends WithPresentationImpl implements
		Media {
	/**
	 * 
	 */
	public MediaAbstractImpl() {
		mLanguage = null;
	}

	private String mLanguage;

	public MediaFactory getMediaFactory() {
		try {
			return getPresentation().getMediaFactory();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public abstract boolean isContinuous();

	public abstract boolean isDiscrete();

	public abstract boolean isSequence();

	public Media copy() {
		return copyProtected();
	}

	protected Media copyProtected() {
		try {
			return exportProtected(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (FactoryCannotCreateTypeException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public Media export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return exportProtected(destPres);
	}

	protected Media exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		Media expMedia;
		try {
			expMedia = destPres.getMediaFactory().createMedia(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (expMedia == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			expMedia.setLanguage(getLanguage());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return expMedia;
	}

	public void setLanguage(String lang)
			throws MethodParameterIsEmptyStringException {
		if (lang == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		String prevlang = mLanguage;
		mLanguage = lang;
		if (prevlang != mLanguage)
			try {
				notifyListeners(new LanguageChangedEvent(this, mLanguage,
						prevlang));
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
	}

	public String getLanguage() {
		return mLanguage;
	}

	@Override
	protected void clear() {
		mLanguage = null;
		super.clear();
	}

	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String lang = source.getAttribute("language");
		if (lang != null)
			lang = lang.trim();
		if (lang == "")
			lang = null;
		try {
			setLanguage(lang);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		super.xukInAttributes(source);
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (getLanguage() != null)
			destination.writeAttributeString("language", getLanguage());
		super.xukOutAttributes(destination, baseUri);
	}

	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		if (other == null)
			throw new MethodParameterIsNullException();
		if (this.getClass() != other.getClass())
			return false;
		if (this.getLanguage() != other.getLanguage())
			return false;
		return true;
	}

	protected ChangeNotifier<DataModelChangedEvent> mLanguageChangedEventNotifier = new ChangeNotifierImpl();

	public <K extends LanguageChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		mLanguageChangedEventNotifier.notifyListeners(event);
	}

	public <K extends LanguageChangedEvent> void registerListener(
			ChangeListener<K> listener, Class<K> klass) throws MethodParameterIsNullException {
		mLanguageChangedEventNotifier.registerListener(listener, klass);
	}

	public <K extends LanguageChangedEvent> void unregisterListener(
			ChangeListener<K> listener, Class<K> klass) throws MethodParameterIsNullException {
		mLanguageChangedEventNotifier.unregisterListener(listener, klass);
	}
}
